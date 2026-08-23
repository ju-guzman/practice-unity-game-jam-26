namespace Interaction
{
    public class CollectableItem : InteractableObject
    {
        public override void Interact()
        {
            base.Interact();
            gameObject.SetActive(false);
        }
    }
}
