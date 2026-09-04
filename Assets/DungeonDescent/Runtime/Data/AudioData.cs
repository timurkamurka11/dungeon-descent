using UnityEngine;
namespace DungeonDescent.Data
{
    [CreateAssetMenu(menuName="Dungeon Descent/Audio Data")]
    public sealed class AudioData:ScriptableObject{public string Id;public AudioClip Clip;[Range(0,1)]public float Volume=1f;public bool Loop;}
}
