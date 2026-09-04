using System;
using UnityEngine;

namespace DungeonDescent.Combat
{
    public sealed class StaminaModel
    {
        public event Action<float, float> Changed;
        public float Max { get; private set; }
        public float Current { get; private set; }
        public float RegenPerSecond { get; set; }
        public float RegenDelay { get; set; }
        private float delayRemaining;

        public StaminaModel(float max, float regenPerSecond, float regenDelay)
        {
            Max = Mathf.Max(1f, max);
            Current = Max;
            RegenPerSecond = Mathf.Max(0f, regenPerSecond);
            RegenDelay = Mathf.Max(0f, regenDelay);
        }

        public bool TrySpend(float amount)
        {
            if (amount <= 0f) return true;
            if (Current + 0.001f < amount) return false;
            Current = Mathf.Max(0f, Current - amount);
            delayRemaining = RegenDelay;
            Changed?.Invoke(Current, Max);
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f || Current >= Max) return;
            if (delayRemaining > 0f)
            {
                delayRemaining -= deltaTime;
                if (delayRemaining > 0f) return;
            }
            var before = Current;
            Current = Mathf.Min(Max, Current + RegenPerSecond * deltaTime);
            if (Mathf.Abs(Current - before) > 0.001f) Changed?.Invoke(Current, Max);
        }

        public void SetMax(float max, bool refill)
        {
            Max = Mathf.Max(1f, max);
            Current = refill ? Max : Mathf.Min(Current, Max);
            Changed?.Invoke(Current, Max);
        }
    }
}
