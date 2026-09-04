using UnityEngine;

namespace DungeonDescent.UI
{
    public static class HudValue
    {
        public static float Normalized(float current, float maximum)
        {
            if (maximum <= 0f) return 0f;
            return Mathf.Clamp01(current / maximum);
        }
    }
}
