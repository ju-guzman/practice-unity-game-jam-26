using System.Collections;
using System.Collections.Generic;
using Inputs;
using UI;
using UnityEngine;

namespace Core
{
    public class RoomCameraNavigator : MonoBehaviour
    {
        [SerializeField] private ScreenFader screenFader;
        [SerializeField] private float fadeOutDuration = 0.25f;
        [SerializeField] private float fadeInDuration = 0.25f;

        private IReadOnlyList<Transform> _viewPoints;
        private int _currentIndex;
        private Coroutine _transitionRoutine;

        private void OnEnable()
        {
            InputManager.Instance.NextRoomRequested += Next;
            InputManager.Instance.PreviousRoomRequested += Previous;
        }

        private void OnDisable()
        {
            InputManager.Instance.NextRoomRequested -= Next;
            InputManager.Instance.PreviousRoomRequested -= Previous;
        }

        public void Initialize(IReadOnlyList<Transform> viewPoints)
        {
            _viewPoints = viewPoints;
            _currentIndex = 0;
            SnapToCurrent();
        }

        private void Next()
        {
            MoveTo(_currentIndex + 1);
        }

        private void Previous()
        {
            MoveTo(_currentIndex - 1);
        }

        private void MoveTo(int index)
        {
            if (_viewPoints == null || _viewPoints.Count == 0 || _transitionRoutine != null)
            {
                return;
            }

            _currentIndex = (index % _viewPoints.Count + _viewPoints.Count) % _viewPoints.Count;
            _transitionRoutine = StartCoroutine(TransitionTo(_viewPoints[_currentIndex]));
        }

        private IEnumerator TransitionTo(Transform target)
        {
            yield return Fade(0f, 1f, fadeOutDuration);

            SnapTo(target);

            yield return Fade(1f, 0f, fadeInDuration);

            _transitionRoutine = null;
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

        private void SnapToCurrent()
        {
            if (_viewPoints == null || _viewPoints.Count == 0)
            {
                return;
            }

            SnapTo(_viewPoints[_currentIndex]);
        }

        private void SnapTo(Transform target)
        {
            if (!target)
            {
                return;
            }

            var position = target.position;
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }
    }
}
