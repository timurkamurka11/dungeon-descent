using DungeonDescent.Presentation;
using UnityEngine;

namespace DungeonDescent.Enemies
{
    public enum EnemyArchetype { GraveRat, HollowSkeleton, CryptCrawler, Cultist, EliteSkeleton }
    public static class EnemyFactory
    {
        public static GameObject Spawn(EnemyArchetype type,Vector3 position,Transform parent=null)
        {
            var go=new GameObject(type.ToString());if(parent!=null)go.transform.SetParent(parent);go.transform.position=position;
            var body=go.AddComponent<CapsuleCollider>();body.radius=type==EnemyArchetype.GraveRat?.40f:.46f;body.height=type==EnemyArchetype.GraveRat?.70f:1.72f;body.center=Vector3.up*(body.height*.5f);
            var health=go.AddComponent<EnemyHealth>();CharacterVisualRig rig;EnemyBrain brain;float hp;
            switch(type)
            {
                case EnemyArchetype.GraveRat:rig=VisualFactory.BuildRat(go.transform);brain=go.AddComponent<GraveRatBrain>();brain.MoveSpeed=5.0f;brain.AttackRange=1.1f;brain.AttackDamage=9f;brain.AttackCooldown=1.25f;brain.EssenceReward=4;hp=38f;break;
                case EnemyArchetype.CryptCrawler:rig=VisualFactory.BuildCrawler(go.transform);brain=go.AddComponent<EnemyBrain>();brain.MoveSpeed=4.4f;brain.AttackRange=1.35f;brain.AttackDamage=14f;brain.EssenceReward=8;hp=68f;break;
                case EnemyArchetype.Cultist:rig=VisualFactory.BuildCultist(go.transform);brain=go.AddComponent<CultistBrain>();brain.MoveSpeed=2.8f;brain.AttackRange=7f;brain.DetectionRange=13f;brain.AttackCooldown=2.1f;brain.EssenceReward=14;hp=85f;break;
                case EnemyArchetype.EliteSkeleton:rig=VisualFactory.BuildSkeleton(go.transform,true);brain=go.AddComponent<EnemyBrain>();brain.MoveSpeed=3.8f;brain.AttackDamage=28f;brain.AttackCooldown=1.15f;brain.EssenceReward=38;hp=230f;go.transform.localScale=Vector3.one*1.18f;break;
                default:rig=VisualFactory.BuildSkeleton(go.transform);brain=go.AddComponent<EnemyBrain>();brain.MoveSpeed=3.25f;brain.AttackDamage=17f;brain.EssenceReward=10;hp=92f;break;
            }
            brain.Configure(hp,rig);return go;
        }
    }
}
