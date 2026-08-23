using Core;
using UnityEngine;

namespace Interaction
{
    public class CollectableItem : InteractableObject
    {
        [SerializeField] private float timeBonusSeconds = 15f;

        public override void Interact()
        {
            base.Interact();

            if (GameManager.Instance)
            {
                GameManager.Instance.AddTime(timeBonusSeconds);
            }

            gameObject.SetActive(false);
        }
    }
}
