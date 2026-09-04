using UnityEngine;
namespace DungeonDescent.Data
{
    [CreateAssetMenu(menuName="Dungeon Descent/Dungeon Biome")]
    public sealed class DungeonBiomeData:ScriptableObject{public string Id;public string DisplayName;public Color FogColor=Color.gray;public float FogDensity=.018f;public string AmbienceClip="dungeon_wind";}
}
