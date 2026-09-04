using System.Collections;
using DungeonDescent.Combat;
using DungeonDescent.Presentation;
using UnityEngine;

namespace DungeonDescent.Enemies
{
    public sealed class CultistBrain : EnemyBrain
    {
        protected override IEnumerator AttackRoutine()
        {
            state=EnemyState.Attack;nextAttackAt=Time.time+AttackCooldown;
            yield return new WaitForSeconds(.48f);
            if(player!=null&&playerVitals!=null)
            {
                var orb=new GameObject("Cultist Arcane Bolt");orb.transform.position=transform.position+Vector3.up*1.25f;var mf=orb.AddComponent<MeshFilter>();mf.sharedMesh=ProceduralMeshFactory.Sphere;var mr=orb.AddComponent<MeshRenderer>();mr.sharedMaterial=MaterialLibrary.MagicBlue;
                var projectile=orb.AddComponent<EnemyProjectile>();projectile.Configure(playerVitals,18f,7.5f,4f);
            }
            state=EnemyState.Recover;yield return new WaitForSeconds(.4f);state=EnemyState.Chase;
        }
    }

    public sealed class EnemyProjectile : MonoBehaviour
    {
        private PlayerVitals target;private float damage,speed,dieAt;
        public void Configure(PlayerVitals t,float d,float s,float life){target=t;damage=d;speed=s;dieAt=Time.time+life;transform.localScale=Vector3.one*.28f;}
        private void Update(){if(target==null||Time.time>=dieAt){Destroy(gameObject);return;}var point=target.transform.position+Vector3.up*.9f;var dir=(point-transform.position);if(dir.magnitude<.55f){target.ReceiveDamage(new DamageInfo(damage,point,dir.normalized,gameObject,5f));Destroy(gameObject);return;}transform.position+=dir.normalized*speed*Time.deltaTime;}
    }
}
