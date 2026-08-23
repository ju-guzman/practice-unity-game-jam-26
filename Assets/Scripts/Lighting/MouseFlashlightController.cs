using Inputs;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils;

namespace Lighting
{
    [RequireComponent(typeof(Light2D))]
    public class MouseFlashlightController : MonoBehaviour
    {
        [SerializeField] private FlashlightSettings settings;

        private Camera _targetCamera;
        private Light2D _light;
        private Vector2 _velocity;
        private Vector2 _currentPosition;

        private void Awake()
        {
            _light = GetComponent<Light2D>();
            
            _targetCamera = Camera.main;
            _currentPosition = transform.position;
        }

        private void Start()
        {
            ApplySettings();
        }

        private void OnEnable()
        {
            InputManager.Instance.PointerMoved += OnPointerMoved;
        }

        private void OnDisable()
        {
            InputManager.Instance.PointerMoved -= OnPointerMoved;
        }

        private Vector2 _targetWorldPosition;

        private void OnPointerMoved(Vector2 screenPosition)
        {
            var z = settings ? settings.worldZ : 0f;
            _targetWorldPosition = ScreenToWorld.FromMouse(_targetCamera, screenPosition, z);
        }

        private void LateUpdate()
        {
            if (!_targetCamera)
            {
                return;
            }

            var smoothTime = settings ? settings.followSmoothTime : 0f;
            _currentPosition = smoothTime > 0f
                ? Vector2.SmoothDamp(_currentPosition, _targetWorldPosition, ref _velocity, smoothTime)
                : _targetWorldPosition;

            transform.position = new Vector3(_currentPosition.x, _currentPosition.y, transform.position.z);
        }

        public void SetSettings(FlashlightSettings newSettings)
        {
            settings = newSettings;
            ApplySettings();
        }

        public void SetIntensity(float intensity)
        {
            _light.intensity = intensity;
        }

        private void ApplySettings()
        {
            if (!settings || !_light)
            {
                return;
            }

            _light.pointLightInnerRadius = settings.innerRadius;
            _light.pointLightOuterRadius = settings.outerRadius;
            _light.color = settings.color;
            _light.intensity = settings.intensity;
        }
    }
}