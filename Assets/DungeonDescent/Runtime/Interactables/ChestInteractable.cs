using System.Collections;
using DungeonDescent.Audio;
using DungeonDescent.Core;
using DungeonDescent.Presentation;
using UnityEngine;

namespace DungeonDescent.Interactables
{
    public sealed class ChestInteractable : MonoBehaviour,IInteractable
    {
        private Transform lid;private bool opened;private int essence,gold;
        public string Prompt=>"[E]  OPEN ANCIENT CHEST";public bool CanInteract=>!opened;
        public void Configure(Transform lidTransform,int essenceReward,int goldReward){lid=lidTransform;essence=essenceReward;gold=goldReward;}
        public void Interact(){if(!opened)StartCoroutine(OpenRoutine());}
        private IEnumerator OpenRoutine(){opened=true;AudioManager.Instance?.PlaySfx("loot",1f);var start=lid.localRotation;var t=0f;while(t<.65f){t+=Time.deltaTime;lid.localRotation=start*Quaternion.Euler(-105f*Mathf.SmoothStep(0,1,t/.65f),0,0);yield return null;}GameSession.Instance?.AddEssence(essence);GameSession.Instance?.AddGold(gold);var glow=VisualFactory.Form("Loot Glow",transform,ProceduralMeshFactory.Sphere,MaterialLibrary.MagicBlue,new Vector3(0,.9f,0),Vector3.one*.45f,Vector3.zero);Destroy(glow,2.5f);}
    }
}
