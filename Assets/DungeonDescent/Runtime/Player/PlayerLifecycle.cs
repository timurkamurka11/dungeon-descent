using System.Collections;
using DungeonDescent.Combat;
using DungeonDescent.UI;
using DungeonDescent.World;
using UnityEngine;

namespace DungeonDescent.Player
{
    [RequireComponent(typeof(PlayerVitals))]
    public sealed class PlayerLifecycle:MonoBehaviour
    {
        private PlayerVitals vitals;private PlayerController controller;private PlayerAnimationController animationController;
        private void Awake(){vitals=GetComponent<PlayerVitals>();controller=GetComponent<PlayerController>();animationController=GetComponent<PlayerAnimationController>();}
        private void OnEnable(){vitals.Died+=OnDied;}private void OnDisable(){vitals.Died-=OnDied;}
        private void OnDied(){StartCoroutine(DeathRoutine());}
        private IEnumerator DeathRoutine(){controller?.SetMovementLocked(true);animationController?.PlayDeath();GameUI.Instance?.ShowDeath();yield return new WaitForSecondsRealtime(2.25f);DungeonWorldBuilder.Instance?.ReturnPlayerToSafeRoom(true);GetComponent<PlayerConsumables>()?.Refill();animationController?.ResetPose();GameUI.Instance?.HideDeath();yield return new WaitForSecondsRealtime(.15f);controller?.SetMovementLocked(false);}
    }
}
