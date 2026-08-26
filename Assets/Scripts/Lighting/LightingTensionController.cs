using Core;
using UnityEngine;

namespace Lighting
{
    /// <summary>
    /// Drives ambient light, vignette, bloom and color grading from GameManager.Value, the same
    /// way MusicTensionController drives the adaptive music - continuous across the full 0-1
    /// range, easing toward each target instead of snapping.
    /// </summary>
    public class LightingTensionController : MonoBehaviour
    {
        [SerializeField] private LightingTensionConfig config;
        [SerializeField] private SceneLightingRig lightingRig;
        [SerializeField] private PostProcessLightingController postProcess;

        private float _currentAmbient;
        private float _currentVignette;
        private float _currentBloom;
        private float _currentSaturation;
        private float _currentPostExposure;
        private Color _currentColorFilter;

        private void Start()
        {
            if (!config)
            {
                return;
            }

            _currentAmbient = config.ambientIntensityAtFull;
            _currentVignette = config.vignetteIntensityAtFull;
            _currentBloom = config.bloomIntensityAtFull;
            _currentSaturation = config.saturationAtFull;
            _currentPostExposure = config.postExposureAtFull;
            _currentColorFilter = config.colorFilterAtFull;
        }

        private void Update()
        {
            if (!config || !GameManager.Instance)
            {
                return;
            }

            var value = GameManager.Instance.Value;
            var step = config.smoothing * Time.deltaTime;
            var colorStep = Mathf.Clamp01(step);

            _currentAmbient = Mathf.MoveTowards(_currentAmbient,
                Mathf.Lerp(config.ambientIntensityAtEmpty, config.ambientIntensityAtFull, value), step);
            _currentVignette = Mathf.MoveTowards(_currentVignette,
                Mathf.Lerp(config.vignetteIntensityAtEmpty, config.vignetteIntensityAtFull, value), step);
            _currentBloom = Mathf.MoveTowards(_currentBloom,
                Mathf.Lerp(config.bloomIntensityAtEmpty, config.bloomIntensityAtFull, value), step);
            _currentSaturation = Mathf.MoveTowards(_currentSaturation,
                Mathf.Lerp(config.saturationAtEmpty, config.saturationAtFull, value), step * 100f);
            _currentPostExposure = Mathf.MoveTowards(_currentPostExposure,
                Mathf.Lerp(config.postExposureAtEmpty, config.postExposureAtFull, value), step);
            _currentColorFilter = Color.Lerp(_currentColorFilter,
                Color.Lerp(config.colorFilterAtEmpty, config.colorFilterAtFull, value), colorStep);

            if (lightingRig)
            {
                lightingRig.SetAmbientIntensity(_currentAmbient);
            }

            if (!postProcess) return;
            postProcess.SetVignetteIntensity(_currentVignette);
            postProcess.SetBloomIntensity(_currentBloom);
            postProcess.SetSaturation(_currentSaturation);
            postProcess.SetPostExposure(_currentPostExposure);
            postProcess.SetColorFilter(_currentColorFilter);
        }
    }
}
