using DungeonDescent.Audio;
using DungeonDescent.Combat;
using DungeonDescent.Core;
using UnityEngine;
namespace DungeonDescent.Interactables
{
    public sealed class RestPointInteractable:MonoBehaviour,IInteractable
    {public string Prompt=>"[E]  REST AND SAVE";public bool CanInteract=>true;public void Interact(){var p=Object.FindFirstObjectByType<PlayerVitals>();p?.RestoreForSafeRoom();GameSession.Instance?.Persist();AudioManager.Instance?.PlaySfx("heal",.8f);}}
}
