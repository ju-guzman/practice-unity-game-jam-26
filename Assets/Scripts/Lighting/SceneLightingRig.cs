using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Lighting
{
    public class SceneLightingRig : MonoBehaviour
    {
        [SerializeField] private Light2D globalLight;

        public void SetAmbientIntensity(float intensity)
        {
            if (globalLight)
            {
                globalLight.intensity = intensity;
            }
        }
    }
}
