using System.Collections;
using System.Collections.Generic;
using DungeonDescent.Audio;
using DungeonDescent.Combat;
using DungeonDescent.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonDescent.Player
{
    [RequireComponent(typeof(PlayerVitals))]
    public sealed class PlayerCombat : MonoBehaviour
    {
        private PlayerVitals vitals;
        private PlayerAnimationController animationController;
        private bool attacking;
        private bool blocking;
        private int comboIndex;
        private float comboResetAt;
        private readonly Collider[] hitBuffer = new Collider[48];

        public Transform AttackOrigin { get; private set; }
        public float WeaponDamage => 24f + (GameSession.Instance != null ? GameSession.Instance.Save.WeaponUpgradeLevel * 5f : 0f);

        public void Configure(PlayerAnimationController animationDriver)
        {
            animationController = animationDriver;
        }

        private void Awake()
        {
            vitals = GetComponent<PlayerVitals>();
            var origin = new GameObject("Combat Origin");
            origin.transform.SetParent(transform, false);
            origin.transform.localPosition = new Vector3(0f, .95f, .35f);
            AttackOrigin = origin.transform;
        }

        private void Update()
        {
            if (Mouse.current == null || !vitals.IsAlive) return;
            if (Time.time > comboResetAt) comboIndex = 0;
            var wantBlock = Mouse.current.rightButton.isPressed && !attacking;
            if (wantBlock != blocking)
            {
                blocking = wantBlock;
                vitals.SetBlocking(blocking);
                animationController?.SetBlocking(blocking);
            }
            if (blocking) return;
            if (Mouse.current.leftButton.wasPressedThisFrame && !attacking)
                StartCoroutine(AttackRoutine(false));
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame && !attacking)
                StartCoroutine(AttackRoutine(true));
        }

        private IEnumerator AttackRoutine(bool heavy)
        {
            var staminaCost = heavy ? 31f : 12f;
            if (!vitals.SpendStamina(staminaCost)) yield break;
            attacking = true;
            comboIndex = heavy ? 0 : (comboIndex % 3) + 1;
            animationController?.PlayAttack(heavy, comboIndex);
            AudioManager.Instance?.PlaySfx("sword_swing", heavy ? 1f : .8f);
            yield return new WaitForSeconds(heavy ? .30f : .18f);
            ResolveAttack(heavy);
            yield return new WaitForSeconds(heavy ? .48f : .26f);
            animationController?.ResetPose();
            comboResetAt = Time.time + .65f;
            attacking = false;
        }

        private void ResolveAttack(bool heavy)
        {
            var reach = heavy ? 2.85f : 2.35f;
            var radius = heavy ? .90f : .76f;
            var minDot = heavy ? -.05f : .10f;
            var forward = transform.forward;
            var origin = transform.position + Vector3.up * .92f;
            var capsuleStart = origin + forward * .35f;
            var capsuleEnd = origin + forward * (reach - .35f) + Vector3.up * .18f;
            var count = Physics.OverlapCapsuleNonAlloc(capsuleStart, capsuleEnd, radius, hitBuffer, ~0, QueryTriggerInteraction.Ignore);
            var alreadyHit = new HashSet<IDamageable>();

            for (var i = 0; i < count; i++)
            {
                var collider = hitBuffer[i];
                if (collider == null || collider.transform.IsChildOf(transform)) continue;
                if (!MeleeTargeting.IsInsideMeleeArc(transform.position, forward, collider.bounds.center, reach + .25f, minDot)) continue;

                IDamageable damageable = null;
                var behaviours = collider.GetComponentsInParent<MonoBehaviour>(true);
                for (var b = 0; b < behaviours.Length; b++)
                {
                    if (behaviours[b] is IDamageable candidate) { damageable = candidate; break; }
                }
                if (damageable == null || damageable.IsDead || !alreadyHit.Add(damageable)) continue;

                var amount = WeaponDamage * (heavy ? 1.8f : 1f + comboIndex * .08f);
                damageable.ReceiveDamage(new DamageInfo(amount, collider.ClosestPoint(origin), forward, gameObject, heavy ? 22f : 8f));
                AudioManager.Instance?.PlaySfx("sword_impact", .9f);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var heavyReach = 2.85f;
            var forward = transform.forward;
            var origin = transform.position + Vector3.up * .92f;
            var a = origin + forward * .35f;
            var b = origin + forward * (heavyReach - .35f) + Vector3.up * .18f;
            Gizmos.DrawWireSphere(a, .90f);
            Gizmos.DrawWireSphere(b, .90f);
            Gizmos.DrawLine(a, b);
        }
#endif
    }
}
