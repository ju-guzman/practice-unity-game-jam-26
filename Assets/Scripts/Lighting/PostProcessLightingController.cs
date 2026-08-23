using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Lighting
{
    /// <summary>
    /// Cosmetic finish on top of the scene's diegetic lighting (vignette, bloom, color grading).
    /// The actual darkness of the scene is driven by SceneLightingRig's Global Light 2D,
    /// not by this Volume.
    /// </summary>
    public class PostProcessLightingController : MonoBehaviour
    {
        [SerializeField] private Volume volume;

        private Vignette _vignette;
        private Bloom _bloom;
        private ColorAdjustments _colorAdjustments;

        private void Awake()
        {
            if (!volume || !volume.profile)
            {
                return;
            }

            volume.profile.TryGet(out _vignette);
            volume.profile.TryGet(out _bloom);
            volume.profile.TryGet(out _colorAdjustments);
        }

        public void SetVignetteIntensity(float intensity)
        {
            if (_vignette)
            {
                _vignette.intensity.value = intensity;
            }
        }

        public void SetBloomIntensity(float intensity)
        {
            if (_bloom)
            {
                _bloom.intensity.value = intensity;
            }
        }
    }
}
