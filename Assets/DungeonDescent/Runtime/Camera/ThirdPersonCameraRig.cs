using DungeonDescent.Core;
using DungeonDescent.Player;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonDescent.CameraSystem
{
    public sealed class ThirdPersonCameraRig:MonoBehaviour
    {
        private PlayerController player;private PlayerLockOn lockOn;private Transform pivot;private float yaw,pitch=12f;private float sensitivity=1f;
        public void Configure(PlayerController owner)
        {
            player=owner;lockOn=owner.GetComponent<PlayerLockOn>();pivot=owner.CameraTarget;sensitivity=GameSession.Instance?.Save.CameraSensitivity??1f;
            var cameraGo=new GameObject("Gameplay Camera");cameraGo.tag="MainCamera";var camera=cameraGo.AddComponent<UnityEngine.Camera>();camera.fieldOfView=58f;camera.nearClipPlane=.08f;camera.farClipPlane=450f;cameraGo.AddComponent<AudioListener>();cameraGo.AddComponent<CinemachineBrain>();
            var vcamGo=new GameObject("Cinemachine Third Person Camera");var vcam=vcamGo.AddComponent<CinemachineCamera>();var target=vcam.Target;target.TrackingTarget=pivot;vcam.Target=target;var follow=vcamGo.AddComponent<CinemachineThirdPersonFollow>();follow.CameraDistance=5.25f;follow.ShoulderOffset=new Vector3(.58f,.72f,0f);follow.VerticalArmLength=.36f;follow.Damping=new Vector3(.12f,.18f,.12f);
            yaw=owner.transform.eulerAngles.y;
        }
        private void LateUpdate()
        {
            if(player==null||pivot==null)return;
            if(lockOn!=null&&lockOn.Locked){var to=lockOn.Target.position-pivot.position;if(to.sqrMagnitude>.1f){var look=Quaternion.LookRotation(to.normalized,Vector3.up).eulerAngles;yaw=Mathf.LerpAngle(yaw,look.y,1f-Mathf.Exp(-6f*Time.deltaTime));pitch=Mathf.LerpAngle(pitch,Mathf.Clamp(look.x>180?look.x-360:look.x,-18f,38f),1f-Mathf.Exp(-6f*Time.deltaTime));}}
            else if(Mouse.current!=null){var delta=Mouse.current.delta.ReadValue();yaw+=delta.x*.08f*sensitivity;pitch=Mathf.Clamp(pitch-delta.y*.065f*sensitivity,-28f,58f);}
            pivot.rotation=Quaternion.Euler(pitch,yaw,0f);
        }
    }
}
