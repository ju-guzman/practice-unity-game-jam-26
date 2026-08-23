using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private const string CameraViewTag = "CameraView";
        private const float SilenceThreshold = 0.001f;

        [Serializable]
        private struct RoomView
        {
            public string sceneName;
            public Vector3 offset;
        }

        [SerializeField] private List<RoomView> roomViews;
        [SerializeField] private RoomCameraNavigator cameraNavigator;

        [Header("Tension Timer")]
        [Tooltip("Total duration of the hidden countdown, in seconds.")]
        [SerializeField] private float totalSeconds = 120f;
        [Tooltip("How many seconds before the timer reaches 0 the tension Value should already be silent (0).")]
        [SerializeField] private float silenceLeadSeconds = 10f;

        public static GameManager Instance { get; private set; }
        
        public static event Action OnMusicSilenced;
        
        public float Value =>
            Mathf.Clamp01((_remainingSeconds - silenceLeadSeconds) / Mathf.Max(totalSeconds - silenceLeadSeconds, 0.0001f));

        public event Action<float> OnValueChanged;

        private Transform[] _cameraViewPoints;
        private int _pendingRoomViews;
        private float _remainingSeconds;
        private float _lastValue;
        private bool _wasSilenced;

        private void Awake()
        {
            Instance = this;
            _remainingSeconds = totalSeconds;
        }

        private void Start()
        {
            _cameraViewPoints = new Transform[roomViews.Count];
            _pendingRoomViews = roomViews.Count;

            for (var i = 0; i < roomViews.Count; i++)
            {
                LoadRoomView(roomViews[i], i);
            }
        }

        private void Update()
        {
            _remainingSeconds = Mathf.Max(_remainingSeconds - Time.deltaTime, 0f);

            var currentValue = Value;
            if (!Mathf.Approximately(currentValue, _lastValue))
            {
                _lastValue = currentValue;
                OnValueChanged?.Invoke(currentValue);
            }
            
            print(currentValue);

            var isSilent = currentValue <= SilenceThreshold;
            switch (isSilent)
            {
                case true when !_wasSilenced:
                    _wasSilenced = true;
                    OnMusicSilenced?.Invoke();
                    break;
                case false:
                    _wasSilenced = false;
                    break;
            }
        }

        public void AddTime(float seconds)
        {
            _remainingSeconds = Mathf.Clamp(_remainingSeconds + seconds, 0f, totalSeconds);
        }

        private void LoadRoomView(RoomView roomView, int index)
        {
            var loadOperation = SceneManager.LoadSceneAsync(roomView.sceneName, LoadSceneMode.Additive);
            if (loadOperation == null)
            {
                Debug.LogError($"Room view scene not found or not in Build Settings: {roomView.sceneName}");
                OnRoomViewReady();
                return;
            }

            loadOperation.completed += _ => OnRoomViewLoaded(roomView, index);
        }

        private void OnRoomViewLoaded(RoomView roomView, int index)
        {
            var scene = SceneManager.GetSceneByName(roomView.sceneName);
            foreach (var root in scene.GetRootGameObjects())
            {
                root.transform.position += roomView.offset;
            }

            _cameraViewPoints[index] = FindCameraView(scene, roomView.sceneName);
            OnRoomViewReady();
        }

        private void OnRoomViewReady()
        {
            if (--_pendingRoomViews > 0)
            {
                return;
            }
            cameraNavigator.Initialize(_cameraViewPoints.Where(t => t).ToArray());
        }

        private static Transform FindCameraView(Scene scene, string sceneName)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                var target = root.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(t => t.CompareTag(CameraViewTag));
                if (target)
                {
                    return target;
                }
            }

            Debug.LogWarning($"No '{CameraViewTag}' tagged object found in scene {sceneName}");
            return null;
        }
    }
}