using System.Collections;
using UnityEngine;

namespace DungeonDescent.Enemies
{
    public sealed class GraveRatBrain : EnemyBrain
    {
        protected override IEnumerator AttackRoutine()
        {
            state=EnemyState.Attack;nextAttackAt=Time.time+AttackCooldown;
            var start=transform.position;var dir=player!=null?Vector3.ProjectOnPlane(player.position-transform.position,Vector3.up).normalized:transform.forward;var t=0f;
            while(t<.22f){t+=Time.deltaTime;transform.position=start+dir*Mathf.Sin((t/.22f)*Mathf.PI*.5f)*1.15f;yield return null;}
            yield return base.AttackRoutine();
        }
    }
}
