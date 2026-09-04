using UnityEngine;

namespace DungeonDescent.Combat
{
    public readonly struct DamageInfo
    {
        public readonly float Amount;
        public readonly Vector3 Point;
        public readonly Vector3 Direction;
        public readonly GameObject Source;
        public readonly float Stagger;

        public DamageInfo(float amount, Vector3 point, Vector3 direction, GameObject source, float stagger = 0f)
        {
            Amount = amount;
            Point = point;
            Direction = direction;
            Source = source;
            Stagger = stagger;
        }
    }

    public interface IDamageable
    {
        bool IsDead { get; }
        void ReceiveDamage(DamageInfo info);
    }
}
