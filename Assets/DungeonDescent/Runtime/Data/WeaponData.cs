using UnityEngine;
namespace DungeonDescent.Data
{
    [CreateAssetMenu(menuName="Dungeon Descent/Weapon Data")]
    public sealed class WeaponData:ScriptableObject{public string Id="runed-longsword";public string DisplayName="Runed Longsword";public float BaseDamage=24f;public float HeavyMultiplier=1.8f;public float Stagger=8f;}
}
