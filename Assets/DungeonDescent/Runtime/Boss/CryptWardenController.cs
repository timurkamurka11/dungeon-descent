using System.Collections;
using DungeonDescent.Audio;
using DungeonDescent.Combat;
using DungeonDescent.Core;
using DungeonDescent.Enemies;
using DungeonDescent.Presentation;
using UnityEngine;

namespace DungeonDescent.Boss
{
    public sealed class CryptWardenController : EnemyBrain
    {
        private bool phaseTwo;
        public override void Configure(float maxHealth, CharacterVisualRig rig)
        {
            base.Configure(maxHealth,rig);AttackDamage=27f;AttackCooldown=1.55f;MoveSpeed=2.8f;AttackRange=2.15f;DetectionRange=18f;EssenceReward=120;
            health.Changed+=(c,m)=>{GameEvents.RaiseBossHealthChanged("THE CRYPT WARDEN",c,m);if(!phaseTwo&&c<=m*.5f)EnterPhaseTwo();};
        }
        protected override void Update(){base.Update();if(!IsDead&&player!=null&&Vector3.Distance(transform.position,player.position)<14f)AudioManager.Instance?.SetMusic(MusicState.Boss,.8f);}
        private void EnterPhaseTwo(){phaseTwo=true;MoveSpeed=3.55f;AttackCooldown=1.05f;AttackDamage=34f;if(visual?.Root!=null)visual.Root.localScale*=1.06f;AudioManager.Instance?.PlaySfx("sword_impact",1f);}
        protected override IEnumerator AttackRoutine()
        {
            if(phaseTwo&&Random.value>.55f)yield return StartCoroutine(ChargeAttack());else yield return StartCoroutine(base.AttackRoutine());
        }
        private IEnumerator ChargeAttack(){state=EnemyState.Attack;nextAttackAt=Time.time+AttackCooldown;yield return new WaitForSeconds(.32f);var dir=player!=null?Vector3.ProjectOnPlane(player.position-transform.position,Vector3.up).normalized:transform.forward;var t=0f;while(t<.5f){t+=Time.deltaTime;transform.position+=dir*8.5f*Time.deltaTime;if(playerVitals!=null&&Vector3.Distance(transform.position,player.position)<1.65f){playerVitals.ReceiveDamage(new DamageInfo(AttackDamage*1.15f,player.position,dir,gameObject,18f));break;}yield return null;}state=EnemyState.Recover;yield return new WaitForSeconds(.4f);state=EnemyState.Chase;}
        protected override void OnDeath(){GameSession.Instance?.MarkBossDefeated();GameSession.Instance?.AddEssence(160);GameSession.Instance?.AddGold(90);GameEvents.RaiseBossEnded();base.OnDeath();}
    }
}
