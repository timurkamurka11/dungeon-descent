using System.Collections;
using DungeonDescent.Audio;
using DungeonDescent.Core;
using UnityEngine;

namespace DungeonDescent.Interactables
{
    public sealed class DungeonDoor : MonoBehaviour, IInteractable
    {
        private Transform left,right;private bool opened,busy;
        public string Prompt=>opened?"The descent is open":"[E]  DESCEND INTO THE DUNGEON";
        public bool CanInteract=>!opened&&!busy;
        public void Configure(Transform leftLeaf,Transform rightLeaf){left=leftLeaf;right=rightLeaf;}
        public void Interact(){if(CanInteract)StartCoroutine(OpenRoutine());}
        private IEnumerator OpenRoutine()
        {
            busy=true;AudioManager.Instance?.PlaySfx("door_creak",1f);AudioManager.Instance?.SetAmbience("dungeon_wind",.34f);var l0=left.localRotation;var r0=right.localRotation;var t=0f;
            while(t<1.8f){t+=Time.deltaTime;var a=Mathf.SmoothStep(0,1,t/1.8f);left.localRotation=l0*Quaternion.Euler(0,-105f*a,0);right.localRotation=r0*Quaternion.Euler(0,105f*a,0);yield return null;}
            opened=true;busy=false;GameSession.Instance?.BeginRun();AudioManager.Instance?.SetMusic(MusicState.Exploration,2.2f);
        }
    }
}
