using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    [RequireComponent(typeof(UIDocument))]
    public class EndScreenController : MonoBehaviour
    {
        [SerializeField] private float storyCycleDuration = 20f;
        [SerializeField] private float restartButtonDelay = 5f;

        private VisualElement _root;
        private SpriteSheetAnimation _storyAnimation;
        private VisualElement _restartOverlay;
        private Button _restartButton;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _root.style.display = DisplayStyle.None;

            _storyAnimation = _root.Q<SpriteSheetAnimation>();
            _restartOverlay = _root.Q<VisualElement>("RestartOverlay");
            _restartButton = _root.Q<Button>("RestartButton");

            if (_restartOverlay != null)
            {
                _restartOverlay.style.display = DisplayStyle.None;
            }

            if (_restartButton != null)
            {
                _restartButton.clicked += HandleRestartClicked;
            }
        }

        private void OnDestroy()
        {
            if (_restartButton != null)
            {
                _restartButton.clicked -= HandleRestartClicked;
            }
        }

        private void OnEnable()
        {
            GameManager.OnGameEnded += HandleGameEnded;
        }

        private void OnDisable()
        {
            GameManager.OnGameEnded -= HandleGameEnded;
        }

        private void HandleGameEnded()
        {
            _root.style.display = DisplayStyle.Flex;

            if (_storyAnimation != null)
            {
                _storyAnimation.CycleDuration = storyCycleDuration;
                _storyAnimation.Playing = true;
            }

            StartCoroutine(ShowRestartButtonAfterDelay(storyCycleDuration + restartButtonDelay));
        }

        private IEnumerator ShowRestartButtonAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_restartOverlay != null)
            {
                _restartOverlay.style.display = DisplayStyle.Flex;
            }
        }

        private void HandleRestartClicked()
        {
            if (GameManager.Instance)
            {
                GameManager.Instance.RestartLevel();
            }
        }
    }
}
