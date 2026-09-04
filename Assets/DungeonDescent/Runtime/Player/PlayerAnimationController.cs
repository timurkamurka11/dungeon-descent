using DungeonDescent.Presentation;
using UnityEngine;

namespace DungeonDescent.Player
{
    public sealed class PlayerAnimationController : MonoBehaviour
    {
        private CharacterVisualRig rig;
        private Vector3 baseScale;
        private Quaternion leftArmBase,rightArmBase,leftLegBase,rightLegBase,swordBase;
        private float locomotionPhase;
        private float attackRemaining;
        private float attackDuration;
        private bool heavyAttack;
        private int attackCombo;
        private float hurtPulse;

        public void Configure(CharacterVisualRig visualRig)
        {
            rig=visualRig;if(rig==null)return;baseScale=rig.Root.localScale;
            leftArmBase=rig.LeftArm!=null?rig.LeftArm.localRotation:Quaternion.identity;rightArmBase=rig.RightArm!=null?rig.RightArm.localRotation:Quaternion.identity;leftLegBase=rig.LeftLeg!=null?rig.LeftLeg.localRotation:Quaternion.identity;rightLegBase=rig.RightLeg!=null?rig.RightLeg.localRotation:Quaternion.identity;swordBase=rig.SwordHand!=null?rig.SwordHand.localRotation:Quaternion.identity;
        }
        public void SetLocomotion(float speed01,bool grounded)
        {
            if(rig?.Root==null)return;locomotionPhase+=Time.deltaTime*Mathf.Lerp(2.2f,9.5f,speed01);var stride=Mathf.Sin(locomotionPhase)*Mathf.Lerp(2f,31f,speed01);var bob=grounded?Mathf.Abs(Mathf.Sin(locomotionPhase))*-.035f*speed01:0f;
            rig.Root.localPosition=Vector3.Lerp(rig.Root.localPosition,new Vector3(0,bob,0),1f-Mathf.Exp(-18f*Time.deltaTime));
            if(attackRemaining>0f){attackRemaining-=Time.deltaTime;AnimateAttack();}
            else
            {
                if(rig.LeftLeg!=null)rig.LeftLeg.localRotation=Quaternion.Slerp(rig.LeftLeg.localRotation,leftLegBase*Quaternion.Euler(stride,0,0),1f-Mathf.Exp(-14f*Time.deltaTime));
                if(rig.RightLeg!=null)rig.RightLeg.localRotation=Quaternion.Slerp(rig.RightLeg.localRotation,rightLegBase*Quaternion.Euler(-stride,0,0),1f-Mathf.Exp(-14f*Time.deltaTime));
                if(rig.LeftArm!=null)rig.LeftArm.localRotation=Quaternion.Slerp(rig.LeftArm.localRotation,leftArmBase*Quaternion.Euler(-stride*.52f,0,0),1f-Mathf.Exp(-11f*Time.deltaTime));
                if(rig.RightArm!=null)rig.RightArm.localRotation=Quaternion.Slerp(rig.RightArm.localRotation,rightArmBase*Quaternion.Euler(stride*.45f,0,0),1f-Mathf.Exp(-11f*Time.deltaTime));
            }
            if(hurtPulse>0f){hurtPulse-=Time.deltaTime;rig.Root.localScale=baseScale*(1f-hurtPulse*.07f);}else rig.Root.localScale=Vector3.Lerp(rig.Root.localScale,baseScale,1f-Mathf.Exp(-15f*Time.deltaTime));
        }
        public void PlayAttack(bool heavy,int comboIndex){heavyAttack=heavy;attackCombo=comboIndex;attackDuration=heavy?.62f:.38f;attackRemaining=attackDuration;}
        private void AnimateAttack()
        {
            if(rig?.RightArm==null)return;var p=1f-Mathf.Clamp01(attackRemaining/Mathf.Max(.01f,attackDuration));var arc=Mathf.Sin(p*Mathf.PI);var yaw=heavyAttack?0f:(attackCombo==2?-42f:42f);var pitch=heavyAttack?Mathf.Lerp(-110f,72f,p):Mathf.Lerp(-55f,70f,p);rig.RightArm.localRotation=Quaternion.Slerp(rig.RightArm.localRotation,rightArmBase*Quaternion.Euler(pitch,yaw,heavyAttack?0f:-28f*arc),1f-Mathf.Exp(-24f*Time.deltaTime));if(rig.Root!=null)rig.Root.localRotation=Quaternion.Euler(0,(heavyAttack?0f:(attackCombo==2?-12f:12f))*arc,0);
        }
        public void PlayDodge(){if(rig?.Root!=null)rig.Root.localRotation=Quaternion.Euler(10f,0,-12f);}
        public void PlayHit(){hurtPulse=.25f;}
        public void ResetPose(){attackRemaining=0;if(rig?.Root!=null){rig.Root.localScale=baseScale;rig.Root.localRotation=Quaternion.identity;}if(rig?.RightArm!=null)rig.RightArm.localRotation=rightArmBase;if(rig?.LeftArm!=null)rig.LeftArm.localRotation=leftArmBase;if(rig?.LeftLeg!=null)rig.LeftLeg.localRotation=leftLegBase;if(rig?.RightLeg!=null)rig.RightLeg.localRotation=rightLegBase;if(rig?.SwordHand!=null)rig.SwordHand.localRotation=swordBase;}
    }
}
