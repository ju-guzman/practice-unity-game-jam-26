using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Lighting
{
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

        public void SetSaturation(float saturation)
        {
            if (_colorAdjustments)
            {
                _colorAdjustments.saturation.value = saturation;
            }
        }

        public void SetColorFilter(Color color)
        {
            if (_colorAdjustments)
            {
                _colorAdjustments.colorFilter.value = color;
            }
        }

        public void SetPostExposure(float exposure)
        {
            if (_colorAdjustments)
            {
                _colorAdjustments.postExposure.value = exposure;
            }
        }
    }
}
