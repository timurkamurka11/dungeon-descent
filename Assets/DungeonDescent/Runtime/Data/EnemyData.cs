using UnityEngine;
namespace DungeonDescent.Data
{
    [CreateAssetMenu(menuName="Dungeon Descent/Enemy Data")]
    public sealed class EnemyData:ScriptableObject{public string Id;public float MaxHealth=80f;public float MoveSpeed=3.4f;public float DetectionRange=10f;public float AttackRange=1.5f;public float AttackDamage=16f;public int EssenceReward=8;}
}
