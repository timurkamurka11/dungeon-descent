using System.Collections;
using System.Collections.Generic;
using DungeonDescent.Audio;
using DungeonDescent.Combat;
using DungeonDescent.Core;
using DungeonDescent.Enemies;
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
        private readonly Collider[] hitBuffer = new Collider[32];

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
            origin.transform.localPosition = new Vector3(0f, 1.05f, .85f);
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
            yield return new WaitForSeconds(heavy ? .34f : .16f);
            ResolveAttack(heavy);
            yield return new WaitForSeconds(heavy ? .52f : .24f);
            animationController?.ResetPose();
            comboResetAt = Time.time + .65f;
            attacking = false;
        }

        private void ResolveAttack(bool heavy)
        {
            var radius = heavy ? 1.45f : 1.15f;
            var count = Physics.OverlapSphereNonAlloc(AttackOrigin.position, radius, hitBuffer, ~0, QueryTriggerInteraction.Collide);
            var alreadyHit = new HashSet<IDamageable>();
            for (var i = 0; i < count; i++)
            {
                var c = hitBuffer[i];
                if (c == null || c.transform.IsChildOf(transform)) continue;
                var to = c.bounds.center - transform.position;
                if (Vector3.Dot(transform.forward, to.normalized) < (heavy ? -.05f : .2f)) continue;
                IDamageable damageable = null;
                var behaviours = c.GetComponentsInParent<MonoBehaviour>(true);
                for (var b = 0; b < behaviours.Length; b++) if (behaviours[b] is IDamageable d) { damageable = d; break; }
                if (damageable == null || !alreadyHit.Add(damageable)) continue;
                var amount = WeaponDamage * (heavy ? 1.8f : 1f + comboIndex * .08f);
                damageable.ReceiveDamage(new DamageInfo(amount, transform.position, transform.forward, gameObject, heavy ? 22f : 8f));
                AudioManager.Instance?.PlaySfx("sword_impact", .9f);
            }
        }
    }
}
