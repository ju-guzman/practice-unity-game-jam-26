using UnityEngine;

namespace Interaction
{
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        public virtual void OnHoverEnter()
        {
        }

        public virtual void OnHoverExit()
        {
        }

        public virtual void Interact()
        {
        }
    }
}
