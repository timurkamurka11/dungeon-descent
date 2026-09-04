using DungeonDescent.Audio;
using DungeonDescent.Core;
using DungeonDescent.World;
using UnityEngine;
namespace DungeonDescent.Interactables
{
    public sealed class ExtractionAltar:MonoBehaviour,IInteractable
    {public string Prompt=>"[E]  CLAIM REWARD AND RETURN HOME";public bool CanInteract=>GameSession.Instance!=null&&GameSession.Instance.BossDefeatedThisRun;public void Interact(){if(!CanInteract)return;GameSession.Instance.FinishRun(true);DungeonWorldBuilder.Instance?.ReturnPlayerToSafeRoom(false);AudioManager.Instance?.SetMusic(MusicState.SafeRoom,2f);}}
}
