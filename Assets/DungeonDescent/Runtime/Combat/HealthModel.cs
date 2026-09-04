using System;
using UnityEngine;

namespace DungeonDescent.Combat
{
    public sealed class HealthModel
    {
        public event Action<float, float> Changed;
        public event Action Died;
        public float Max { get; private set; }
        public float Current { get; private set; }
        public bool IsDead { get; private set; }

        public HealthModel(float max)
        {
            Max = Mathf.Max(1f, max);
            Current = Max;
        }

        public void SetMax(float max, bool refill)
        {
            Max = Mathf.Max(1f, max);
            Current = refill ? Max : Mathf.Min(Current, Max);
            Changed?.Invoke(Current, Max);
        }

        public void ApplyDamage(float amount)
        {
            if (IsDead || amount <= 0f) return;
            Current = Mathf.Max(0f, Current - amount);
            Changed?.Invoke(Current, Max);
            if (Current > 0f) return;
            IsDead = true;
            Died?.Invoke();
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f) return;
            Current = Mathf.Min(Max, Current + amount);
            Changed?.Invoke(Current, Max);
        }

        public void Refill()
        {
            IsDead = false;
            Current = Max;
            Changed?.Invoke(Current, Max);
        }
    }
}
