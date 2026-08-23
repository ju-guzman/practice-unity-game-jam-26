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

        [Serializable]
        private struct RoomView
        {
            public string sceneName;
            public Vector3 offset;
        }

        [SerializeField] private List<RoomView> roomViews;
        [SerializeField] private RoomCameraNavigator cameraNavigator;

        private Transform[] _cameraViewPoints;
        private int _pendingRoomViews;

        private void Start()
        {
            _cameraViewPoints = new Transform[roomViews.Count];
            _pendingRoomViews = roomViews.Count;

            for (var i = 0; i < roomViews.Count; i++)
            {
                LoadRoomView(roomViews[i], i);
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