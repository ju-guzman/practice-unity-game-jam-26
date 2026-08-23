using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Lighting
{
    public class SceneLightingRig : MonoBehaviour
    {
        [SerializeField] private Light2D globalLight;
        [SerializeField] private MouseFlashlightController flashlight;

        public void SetAmbientIntensity(float intensity)
        {
            if (globalLight)
            {
                globalLight.intensity = intensity;
            }
        }

        public void SetFlashlightIntensity(float intensity)
        {
            if (flashlight)
            {
                flashlight.SetIntensity(intensity);
            }
        }

        public void SetFlashlightSettings(FlashlightSettings settings)
        {
            if (flashlight)
            {
                flashlight.SetSettings(settings);
            }
        }
    }
}
