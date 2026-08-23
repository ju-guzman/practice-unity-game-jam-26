using UnityEngine;

namespace Lighting
{
    [CreateAssetMenu(fileName = "FlashlightSettings", menuName = "Game/Lighting/Flashlight Settings")]
    public class FlashlightSettings : ScriptableObject
    {
        [Header("Shape")]
        public float innerRadius = 0.5f;
        public float outerRadius = 4f;

        [Header("Look")]
        public Color color = Color.white;
        public float intensity = 1.5f;

        [Header("Follow")]
        [Tooltip("0 = snap instantly to the mouse, higher = smoother trailing follow.")]
        public float followSmoothTime = 0.05f;

        [Header("Depth")]
        public float worldZ;
    }
}
