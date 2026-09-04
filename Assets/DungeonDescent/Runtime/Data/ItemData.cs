using UnityEngine;
namespace DungeonDescent.Data
{
    public enum ItemRarity{Common,Uncommon,Rare,Epic,Legendary}
    [CreateAssetMenu(menuName="Dungeon Descent/Item Data")]
    public sealed class ItemData:ScriptableObject{public string Id;public string DisplayName;public ItemRarity Rarity;public string Description;public int GoldValue;}
}
