using DungeonDescent.Core;
using DungeonDescent.Interactables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonDescent.Player
{
    public sealed class PlayerInteraction : MonoBehaviour
    {
        private readonly Collider[] buffer = new Collider[24];
        private IInteractable current;

        private void Update()
        {
            current = FindNearest();
            GameEvents.RaiseInteractionPromptChanged(current != null && current.CanInteract ? current.Prompt : string.Empty);
            if (current != null && current.CanInteract && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                current.Interact();
        }

        private IInteractable FindNearest()
        {
            var count = Physics.OverlapSphereNonAlloc(transform.position + Vector3.up * .8f, 2.35f, buffer, ~0, QueryTriggerInteraction.Collide);
            IInteractable best = null; var bestDistance = float.MaxValue;
            for (var i = 0; i < count; i++)
            {
                var c = buffer[i]; if (c == null) continue;
                IInteractable interactable = null;
                var behaviours = c.GetComponentsInParent<MonoBehaviour>(true);
                for (var b = 0; b < behaviours.Length; b++) if (behaviours[b] is IInteractable found) { interactable = found; break; }
                if (interactable == null || !interactable.CanInteract) continue;
                var distance = Vector3.SqrMagnitude(c.transform.position - transform.position);
                if (distance < bestDistance) { bestDistance = distance; best = interactable; }
            }
            return best;
        }

        private void OnDisable() => GameEvents.RaiseInteractionPromptChanged(string.Empty);
    }
}
