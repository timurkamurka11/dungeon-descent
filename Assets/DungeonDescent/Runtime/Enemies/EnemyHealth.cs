using System;
using DungeonDescent.Combat;
using UnityEngine;

namespace DungeonDescent.Enemies
{
    public sealed class EnemyHealth : MonoBehaviour, IDamageable
    {
        private HealthModel model;
        public bool IsDead => model == null || model.IsDead;
        public float Current => model?.Current ?? 0f;
        public float Max => model?.Max ?? 1f;
        public event Action<DamageInfo> Damaged;
        public event Action Died;
        public event Action<float,float> Changed;

        public void Configure(float maximum)
        {
            model = new HealthModel(maximum);
            model.Changed += (c,m) => Changed?.Invoke(c,m);
            model.Died += () => Died?.Invoke();
        }

        public void ReceiveDamage(DamageInfo info)
        {
            if (model == null || model.IsDead) return;
            model.ApplyDamage(info.Amount);
            Damaged?.Invoke(info);
        }
    }
}
