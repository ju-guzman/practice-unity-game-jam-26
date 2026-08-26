using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inputs;
using Interaction;
using UI;
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

        [Header("Restart")]
        [SerializeField] private ScreenFader screenFader;
        [SerializeField] private float restartFadeDuration = 0.3f;

        [Header("Tension Timer")]
        [SerializeField] private float totalSeconds = 120f;
        [SerializeField] private float silenceLeadSeconds = 10f;

        [Header("Pickup Sequence")]
        [SerializeField] private List<string> collectionSequence = new();

        private readonly Dictionary<string, CollectableItem> _collectables = new();
        private int _sequenceIndex;

        public static GameManager Instance { get; private set; }

        public static event Action OnMusicSilenced;
        public static event Action OnGameEnded;
        public static event Action OnGameStarted;

        private bool _hasEnded;
        private bool _gameStarted;

        public float Value =>
            Mathf.Clamp01((_remainingSeconds - silenceLeadSeconds) / Mathf.Max(totalSeconds - silenceLeadSeconds, 0.0001f));

        public event Action<float> OnValueChanged;

        public float CollectionProgress =>
            collectionSequence.Count == 0 ? 0f : (float)_sequenceIndex / collectionSequence.Count;

        public IReadOnlyCollection<CollectableItem> RegisteredCollectables => _collectables.Values;

        private Transform[] _cameraViewPoints;
        private int _pendingRoomViews;
        private float _remainingSeconds;
        private float _lastValue;
        private bool _wasSilenced;
        private bool _isRestarting;
        private bool _timerStopped;

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

            InputManager.Instance.RestartRequested += RestartLevel;
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.RestartRequested -= RestartLevel;
            }
        }

        private void Update()
        {
            if (!_gameStarted)
            {
                return;
            }

            if (!_timerStopped)
            {
                _remainingSeconds = Mathf.Max(_remainingSeconds - Time.deltaTime, 0f);
            }

            var currentValue = Value;
            Debug.Log(Value);
            if (!Mathf.Approximately(currentValue, _lastValue))
            {
                _lastValue = currentValue;
                OnValueChanged?.Invoke(currentValue);
            }

            var isSilent = currentValue <= SilenceThreshold;
            switch (isSilent)
            {
                case true when !_wasSilenced:
                    _wasSilenced = true;
                    _timerStopped = true;
                    OnMusicSilenced?.Invoke();
                    break;
                case false:
                    _wasSilenced = false;
                    break;
            }
        }

        private IEnumerator EndGameAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            EndGame();
        }

        public void AddTime(float seconds)
        {
            _remainingSeconds = Mathf.Clamp(_remainingSeconds + seconds, 0f, totalSeconds);
        }

        public void StartGame()
        {
            if (_gameStarted)
            {
                return;
            }
            _gameStarted = true;
            OnGameStarted?.Invoke();
        }

        public void EndGame()
        {
            if (_hasEnded)
            {
                return;
            }
            _hasEnded = true;
            OnGameEnded?.Invoke();
        }

        public void RestartLevel()
        {
            if (_isRestarting)
            {
                return;
            }

            _isRestarting = true;
            StartCoroutine(RestartRoutine());
        }

        private IEnumerator RestartRoutine()
        {
            yield return Fade(0f, 1f, restartFadeDuration);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            if (!screenFader)
            {
                yield break;
            }

            if (duration <= 0f)
            {
                screenFader.SetOpacity(to);
                yield break;
            }

            var elapsed = 0f;
            screenFader.SetOpacity(from);
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                screenFader.SetOpacity(Mathf.Lerp(from, to, elapsed / duration));
                yield return null;
            }

            screenFader.SetOpacity(to);
        }
        
        public void RegisterCollectable(string itemId, CollectableItem item)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return;
            }

            _collectables[itemId] = item;
            TryUnlockCurrent();
        }

        public void NotifyCollected(string itemId)
        {
            if (_sequenceIndex >= collectionSequence.Count || collectionSequence[_sequenceIndex] != itemId)
            {
                return;
            }

            _sequenceIndex++;

            if (_sequenceIndex >= collectionSequence.Count)
            {
                StartCoroutine(EndGameAfterDelay(5f));
            }
            else
            {
                TryUnlockCurrent();
            }
        }

        private void TryUnlockCurrent()
        {
            if (_sequenceIndex >= collectionSequence.Count)
            {
                return;
            }

            if (_collectables.TryGetValue(collectionSequence[_sequenceIndex], out var current))
            {
                current.Unlock();
            }
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