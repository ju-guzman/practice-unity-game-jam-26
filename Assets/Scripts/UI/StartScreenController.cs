using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    [RequireComponent(typeof(UIDocument))]
    public class StartScreenController : MonoBehaviour
    {
        [SerializeField] private float storyCycleDuration = 6f;
        [SerializeField] private float fadeOutDuration = 1.5f;

        private VisualElement _root;
        private SpriteSheetAnimation _storyAnimation;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _root.style.display = DisplayStyle.Flex;
            _root.style.opacity = 1f;

            _storyAnimation = _root.Q<SpriteSheetAnimation>();
        }

        private void Start()
        {
            if (_storyAnimation != null)
            {
                _storyAnimation.CycleDuration = storyCycleDuration;
                _storyAnimation.Loop = false;
                _storyAnimation.Playing = true;
            }

            StartCoroutine(PlayIntroSequence());
        }

        private IEnumerator PlayIntroSequence()
        {
            yield return new WaitForSeconds(storyCycleDuration);
            yield return Fade(1f, 0f, fadeOutDuration);

            _root.style.display = DisplayStyle.None;

            if (GameManager.Instance)
            {
                GameManager.Instance.StartGame();
            }
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            if (duration <= 0f)
            {
                _root.style.opacity = to;
                yield break;
            }

            var elapsed = 0f;
            _root.style.opacity = from;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _root.style.opacity = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }

            _root.style.opacity = to;
        }
    }
}
