using DungeonDescent.Progression;using UnityEngine;
namespace DungeonDescent.Data
{
    [CreateAssetMenu(menuName="Dungeon Descent/Upgrade Data")]
    public sealed class UpgradeData:ScriptableObject{public UpgradeKind Kind;public string DisplayName;public string Description;public int BaseCost=100;}
}
