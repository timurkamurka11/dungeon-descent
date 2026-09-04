using System.Collections;
using DungeonDescent.Audio;
using DungeonDescent.Combat;
using DungeonDescent.Player;
using DungeonDescent.Presentation;
using UnityEngine;
using UnityEngine.AI;

namespace DungeonDescent.Enemies
{
    public enum EnemyState { Idle, Patrol, Suspicious, Investigate, Chase, Attack, Recover, Stagger, Return, Dead }

    [RequireComponent(typeof(EnemyHealth))]
    public class EnemyBrain : MonoBehaviour
    {
        protected EnemyHealth health;
        protected NavMeshAgent agent;
        protected Transform player;
        protected PlayerVitals playerVitals;
        protected CharacterVisualRig visual;
        protected Vector3 home;
        protected EnemyState state=EnemyState.Idle;
        protected float nextAttackAt;
        protected float nextThinkAt;
        protected float visualPhase;

        public float DetectionRange=10f;
        public float AttackRange=1.55f;
        public float MoveSpeed=3.4f;
        public float AttackDamage=16f;
        public float AttackCooldown=1.5f;
        public int EssenceReward=8;
        public EnemyState State=>state;
        public bool IsDead=>health==null||health.IsDead;

        public virtual void Configure(float maxHealth, CharacterVisualRig rig)
        {
            visual=rig; health=GetComponent<EnemyHealth>(); health.Configure(maxHealth); health.Damaged+=OnDamaged; health.Died+=OnDeath;
            agent=GetComponent<NavMeshAgent>(); if(agent==null)agent=gameObject.AddComponent<NavMeshAgent>();agent.radius=.42f;agent.height=1.75f;agent.speed=MoveSpeed;agent.angularSpeed=540f;agent.acceleration=18f;agent.stoppingDistance=AttackRange*.72f;
            home=transform.position;
        }

        protected virtual void Update()
        {
            if(IsDead)return;
            if(player==null){var p=Object.FindFirstObjectByType<PlayerController>();if(p!=null){player=p.transform;playerVitals=p.GetComponent<PlayerVitals>();}}
            AnimateVisual();
            if(player==null||playerVitals==null||!playerVitals.IsAlive)return;
            var delta=player.position-transform.position;delta.y=0;var distance=delta.magnitude;
            if(state==EnemyState.Stagger||state==EnemyState.Recover)return;
            if(distance<=AttackRange&&Time.time>=nextAttackAt){StartCoroutine(AttackRoutine());return;}
            if(distance<=DetectionRange){state=EnemyState.Chase;MoveToward(player.position);}
            else if(Vector3.Distance(transform.position,home)>1.2f){state=EnemyState.Return;MoveToward(home);}
            else state=EnemyState.Idle;
        }

        protected virtual IEnumerator AttackRoutine()
        {
            state=EnemyState.Attack;nextAttackAt=Time.time+AttackCooldown;
            if(agent!=null&&agent.isOnNavMesh)agent.isStopped=true;
            var windup=.35f;var start=transform.rotation;var target=Quaternion.LookRotation(Vector3.ProjectOnPlane(player.position-transform.position,Vector3.up).normalized,Vector3.up);var t=0f;
            while(t<windup){t+=Time.deltaTime;transform.rotation=Quaternion.Slerp(start,target,t/windup);yield return null;}
            if(playerVitals!=null&&Vector3.Distance(transform.position,player.position)<=AttackRange+0.55f)
                playerVitals.ReceiveDamage(new DamageInfo(AttackDamage,player.position,(player.position-transform.position).normalized,gameObject,8f));
            AudioManager.Instance?.PlaySfx("sword_impact",.45f);
            state=EnemyState.Recover;yield return new WaitForSeconds(.48f);if(agent!=null&&agent.isOnNavMesh)agent.isStopped=false;state=EnemyState.Chase;
        }

        protected void MoveToward(Vector3 point)
        {
            if(agent!=null&&agent.enabled&&agent.isOnNavMesh){agent.isStopped=false;agent.speed=MoveSpeed;agent.SetDestination(point);return;}
            var d=Vector3.ProjectOnPlane(point-transform.position,Vector3.up);if(d.sqrMagnitude<.01f)return;d.Normalize();transform.position+=d*MoveSpeed*Time.deltaTime;transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(d),1f-Mathf.Exp(-8f*Time.deltaTime));
        }

        protected virtual void OnDamaged(DamageInfo info)
        {
            if(IsDead)return;StopAllCoroutines();state=EnemyState.Stagger;if(agent!=null&&agent.isOnNavMesh)agent.isStopped=true;StartCoroutine(StaggerRoutine(info.Stagger));
        }
        private IEnumerator StaggerRoutine(float strength){var d=Mathf.Lerp(.08f,.28f,Mathf.Clamp01(strength/25f));yield return new WaitForSeconds(d);if(!IsDead){if(agent!=null&&agent.isOnNavMesh)agent.isStopped=false;state=EnemyState.Chase;}}
        protected virtual void OnDeath(){state=EnemyState.Dead;if(agent!=null)agent.enabled=false;AudioManager.Instance?.PlaySfx("sword_impact",.7f);DungeonDescent.Core.GameSession.Instance?.AddEssence(EssenceReward);StartCoroutine(DeathRoutine());}
        private IEnumerator DeathRoutine(){var start=transform.localScale;var t=0f;while(t<.55f){t+=Time.deltaTime;transform.localScale=Vector3.Lerp(start,new Vector3(start.x*.8f,.05f,start.z*.8f),t/.55f);transform.Rotate(Vector3.forward,70f*Time.deltaTime);yield return null;}Destroy(gameObject);}
        protected virtual void AnimateVisual(){if(visual?.Root==null)return;var moving=state==EnemyState.Chase||state==EnemyState.Return;visualPhase+=Time.deltaTime*(moving?8f:2.5f);var swing=Mathf.Sin(visualPhase)*(moving?28f:3f);visual.Root.localPosition=Vector3.Lerp(visual.Root.localPosition,new Vector3(0,Mathf.Abs(Mathf.Sin(visualPhase))*(moving?.035f:.012f),0),1f-Mathf.Exp(-12f*Time.deltaTime));if(visual.LeftLeg!=null)visual.LeftLeg.localRotation=Quaternion.Slerp(visual.LeftLeg.localRotation,Quaternion.Euler(swing,0,0),1f-Mathf.Exp(-12f*Time.deltaTime));if(visual.RightLeg!=null)visual.RightLeg.localRotation=Quaternion.Slerp(visual.RightLeg.localRotation,Quaternion.Euler(-swing,0,0),1f-Mathf.Exp(-12f*Time.deltaTime));if(visual.LeftArm!=null)visual.LeftArm.localRotation=Quaternion.Slerp(visual.LeftArm.localRotation,Quaternion.Euler(-swing*.45f,0,0),1f-Mathf.Exp(-10f*Time.deltaTime));}
    }
}
