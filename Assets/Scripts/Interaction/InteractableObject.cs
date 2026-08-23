using UnityEngine;

namespace Interaction
{
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionId;

        public virtual void OnHoverEnter()
        {
        }

        public virtual void OnHoverExit()
        {
        }

        public virtual void Interact()
        {
            Debug.Log($"Interact: {(string.IsNullOrEmpty(interactionId) ? gameObject.name : interactionId)}");
        }
    }
}
