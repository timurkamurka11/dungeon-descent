using System;
using DungeonDescent.Core;
using UnityEngine;

namespace DungeonDescent.Combat
{
    public sealed class PlayerVitals : MonoBehaviour, IDamageable
    {
        private HealthModel health;
        private StaminaModel stamina;
        private float invulnerableUntil;
        private float blockMultiplier = 1f;

        public float CurrentHealth => health?.Current ?? 0f;
        public float MaxHealth => health?.Max ?? 1f;
        public float CurrentStamina => stamina?.Current ?? 0f;
        public float MaxStamina => stamina?.Max ?? 1f;
        public bool IsAlive => health != null && health.Current > 0f;
        public bool IsDead => !IsAlive;
        public bool IsInvulnerable => Time.time < invulnerableUntil;
        public event Action Died;

        public void Configure(float maxHealth, float maxStamina)
        {
            health = new HealthModel(maxHealth);
            stamina = new StaminaModel(maxStamina, 22f, .7f);
            health.Changed += (_, __) => GameEvents.RaisePlayerHealthChanged(CurrentHealth, MaxHealth);
            health.Died += HandleDeath;
            stamina.Changed += (_, __) => GameEvents.RaisePlayerStaminaChanged(CurrentStamina, MaxStamina);
            Publish();
        }

        private void Update()
        {
            if (stamina == null || !IsAlive) return;
            stamina.Tick(Time.deltaTime);
        }

        public bool SpendStamina(float amount) => stamina != null && stamina.TrySpend(amount);
        public void SetInvulnerable(float seconds) => invulnerableUntil = Mathf.Max(invulnerableUntil, Time.time + Mathf.Max(0f, seconds));
        public void SetBlocking(bool blocking) => blockMultiplier = blocking ? .22f : 1f;

        public void ReceiveDamage(DamageInfo info)
        {
            if (!IsAlive || IsInvulnerable) return;
            var damage = Mathf.Max(0f, info.Amount * blockMultiplier);
            if (blockMultiplier < 1f && !SpendStamina(Mathf.Max(8f, info.Amount * .35f)))
                blockMultiplier = 1f;
            health.ApplyDamage(damage);
        }

        public bool Heal(float amount)
        {
            if (!IsAlive || amount <= 0f || CurrentHealth >= MaxHealth) return false;
            health.Heal(amount);
            return true;
        }

        public void RestoreForSafeRoom()
        {
            if (health == null || stamina == null) return;
            health.Refill();
            stamina.SetMax(MaxStamina, true);
            Publish();
        }

        private void HandleDeath()
        {
            Died?.Invoke();
        }

        private void Publish()
        {
            GameEvents.RaisePlayerHealthChanged(CurrentHealth, MaxHealth);
            GameEvents.RaisePlayerStaminaChanged(CurrentStamina, MaxStamina);
        }
    }
}
