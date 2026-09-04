using System;using UnityEngine;
namespace DungeonDescent.Data
{
    [Serializable] public struct LootEntry{public ItemData Item;[Min(0f)]public float Weight;}
    [CreateAssetMenu(menuName="Dungeon Descent/Loot Table")]
    public sealed class LootTable:ScriptableObject{public LootEntry[] Entries;public int EssenceMin=4;public int EssenceMax=14;public int GoldMin=2;public int GoldMax=10;}
}
