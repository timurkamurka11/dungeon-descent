using UnityEngine;

namespace DungeonDescent.Combat
{
    public static class MeleeTargeting
    {
        public static bool IsInsideMeleeArc(Vector3 origin, Vector3 forward, Vector3 target, float maxDistance, float minDot)
        {
            var delta = target - origin;
            delta.y = 0f;
            if (delta.sqrMagnitude > maxDistance * maxDistance) return false;
            if (delta.sqrMagnitude < 0.0001f) return true;

            var flatForward = forward;
            flatForward.y = 0f;
            if (flatForward.sqrMagnitude < 0.0001f) flatForward = Vector3.forward;
            flatForward.Normalize();
            return Vector3.Dot(flatForward, delta.normalized) >= minDot;
        }
    }
}
