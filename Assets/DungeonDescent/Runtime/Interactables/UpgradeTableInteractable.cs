using DungeonDescent.UI;
using UnityEngine;
namespace DungeonDescent.Interactables
{
    public sealed class UpgradeTableInteractable:MonoBehaviour,IInteractable
    {public string Prompt=>"[E]  OPEN PERMANENT UPGRADES";public bool CanInteract=>true;public void Interact(){GameUI.Instance?.OpenUpgradeMenu();}}
}
