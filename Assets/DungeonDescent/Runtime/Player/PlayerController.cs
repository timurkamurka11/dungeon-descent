using System.Collections;
using DungeonDescent.Combat;
using DungeonDescent.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonDescent.Player
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerVitals))]
    public sealed class PlayerController : MonoBehaviour
    {
        private CharacterController controller;
        private PlayerVitals vitals;
        private PlayerAnimationController animationController;
        private Transform cameraTransform;
        private float verticalVelocity;
        private bool dodging;
        private bool movementLocked;
        private Vector3 lastMoveDirection = Vector3.forward;

        public float WalkSpeed = 3.1f;
        public float JogSpeed = 5.1f;
        public float SprintSpeed = 7.2f;
        public Transform CameraTarget { get; private set; }
        public Vector3 FacingDirection => transform.forward;
        public Vector3 LastMoveDirection => lastMoveDirection;
        public bool MovementLocked => movementLocked;

        public void Configure(PlayerAnimationController animationDriver)
        {
            animationController = animationDriver;
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            vitals = GetComponent<PlayerVitals>();
            controller.radius = .42f;
            controller.height = 1.82f;
            controller.center = new Vector3(0f, .91f, 0f);
            controller.stepOffset = .42f;
            controller.slopeLimit = 48f;
            var target = new GameObject("Camera Target");
            target.transform.SetParent(transform, false);
            target.transform.localPosition = new Vector3(0f, 1.48f, 0f);
            CameraTarget = target.transform;
        }

        private void Start() => cameraTransform = Camera.main != null ? Camera.main.transform : null;

        private void Update()
        {
            if (Keyboard.current == null || controller == null || !vitals.IsAlive) return;
            if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
            if (Keyboard.current.spaceKey.wasPressedThisFrame) TryDodge();
            if (!movementLocked && !dodging) Move();
            else ApplyGravityOnly();
        }

        private void Move()
        {
            var x = (Keyboard.current.dKey.isPressed ? 1f : 0f) - (Keyboard.current.aKey.isPressed ? 1f : 0f);
            var z = (Keyboard.current.wKey.isPressed ? 1f : 0f) - (Keyboard.current.sKey.isPressed ? 1f : 0f);
            var input = Vector2.ClampMagnitude(new Vector2(x, z), 1f);
            Vector3 forward = cameraTransform != null ? Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized : Vector3.forward;
            Vector3 right = cameraTransform != null ? Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized : Vector3.right;
            var direction = (forward * input.y + right * input.x);
            if (direction.sqrMagnitude > .01f)
            {
                lastMoveDirection = direction.normalized;
                var targetRotation = Quaternion.LookRotation(lastMoveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f - Mathf.Exp(-13f * Time.deltaTime));
            }
            var sprint = Keyboard.current.leftShiftKey.isPressed && input.y > .1f && vitals.CurrentStamina > 2f;
            var speed = sprint ? SprintSpeed : (input.magnitude > .45f ? JogSpeed : WalkSpeed);
            if (sprint && direction.sqrMagnitude > .01f && !vitals.SpendStamina(12f * Time.deltaTime)) speed = JogSpeed;
            verticalVelocity = controller.isGrounded ? -2f : verticalVelocity + Physics.gravity.y * Time.deltaTime;
            var velocity = direction * speed + Vector3.up * verticalVelocity;
            controller.Move(velocity * Time.deltaTime);
            animationController?.SetLocomotion(Mathf.Clamp01(direction.magnitude * speed / SprintSpeed), controller.isGrounded);
        }

        private void ApplyGravityOnly()
        {
            verticalVelocity = controller.isGrounded ? -2f : verticalVelocity + Physics.gravity.y * Time.deltaTime;
            controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        }

        public bool TryDodge()
        {
            if (dodging || movementLocked || !vitals.IsAlive || !vitals.SpendStamina(24f)) return false;
            StartCoroutine(DodgeRoutine());
            return true;
        }

        private IEnumerator DodgeRoutine()
        {
            dodging = true;
            vitals.SetInvulnerable(.36f);
            animationController?.PlayDodge();
            var direction = lastMoveDirection.sqrMagnitude > .01f ? lastMoveDirection : transform.forward;
            var elapsed = 0f;
            const float duration = .42f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var normalized = elapsed / duration;
                var speed = Mathf.Lerp(10.5f, 2.2f, normalized);
                controller.Move(direction * speed * Time.deltaTime);
                yield return null;
            }
            animationController?.ResetPose();
            dodging = false;
        }

        public void SetMovementLocked(bool locked) => movementLocked = locked;
        public void TeleportSafe(Vector3 position, Quaternion rotation)
        {
            controller.enabled = false;
            transform.SetPositionAndRotation(position, rotation);
            controller.enabled = true;
            verticalVelocity = 0f;
        }
    }
}
