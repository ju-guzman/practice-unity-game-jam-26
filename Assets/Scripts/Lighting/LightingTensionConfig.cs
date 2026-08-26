using UnityEngine;

namespace Lighting
{
    [CreateAssetMenu(fileName = "LightingTensionConfig", menuName = "Game/Lighting/Lighting Tension Config")]
    public class LightingTensionConfig : ScriptableObject
    {
        [Header("Ambient Light (Value: 0 -> 1)")]
        public float ambientIntensityAtEmpty = 0.3f;
        public float ambientIntensityAtFull = 1f;

        [Header("Vignette (Value: 0 -> 1)")]
        public float vignetteIntensityAtEmpty = 0.4f;
        public float vignetteIntensityAtFull = 0f;

        [Header("Bloom (Value: 0 -> 1)")]
        public float bloomIntensityAtEmpty = 0.6f;
        public float bloomIntensityAtFull = 0.2f;

        [Header("Color Grading (Value: 0 -> 1)")]
        public float saturationAtEmpty = -40f;
        public float saturationAtFull;

        public Color colorFilterAtEmpty = new(0.6f, 0.7f, 0.85f);
        public Color colorFilterAtFull = Color.white;

        public float postExposureAtEmpty = -0.5f;
        public float postExposureAtFull;

        [Header("Smoothing")]
        public float smoothing = 2f;
    }
}
