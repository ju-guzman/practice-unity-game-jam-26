using UnityEngine;

namespace Interaction
{
    public static class InteractionLayers
    {
        private const string InteractableLayerName = "Interactable";

        public static LayerMask InteractableMask => LayerMask.GetMask(InteractableLayerName);
    }
}
