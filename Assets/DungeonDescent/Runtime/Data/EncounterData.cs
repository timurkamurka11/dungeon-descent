using DungeonDescent.Enemies;using UnityEngine;
namespace DungeonDescent.Data
{
    [CreateAssetMenu(menuName="Dungeon Descent/Encounter Data")]
    public sealed class EncounterData:ScriptableObject{public string Id;public EnemyArchetype[] Enemies;public bool LockDoors;public int RewardEssence;}
}
