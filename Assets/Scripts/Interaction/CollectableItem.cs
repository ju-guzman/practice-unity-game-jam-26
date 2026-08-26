using System.Collections;
using Core;
using UnityEngine;

namespace Interaction
{
    public class CollectableItem : InteractableObject
    {
        [Header("Sequence")]
        [SerializeField]
        private string itemId;

        [SerializeField] private float timeBonusSeconds = 15f;
        [SerializeField] private float defaultFadeDuration = 1f;

        private SpriteRenderer _spriteRenderer;

        private bool _isUnlocked;
        private bool _isConsumed;
        private bool _isFading;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            if (GameManager.Instance)
            {
                GameManager.Instance.RegisterCollectable(itemId, this);
            }
        }

        public void Unlock()
        {
            _isUnlocked = true;
        }

        public override void Interact()
        {
            if (!_isUnlocked || _isConsumed)
            {
                return;
            }

            _isConsumed = true;
            base.Interact();

            if (GameManager.Instance)
            {
                GameManager.Instance.AddTime(timeBonusSeconds);
                GameManager.Instance.NotifyCollected(itemId);
            }

            if (!_isFading)
            {
                BeginFade(defaultFadeDuration);
            }
        }

        public void BeginFade(float duration)
        {
            if (_isFading)
            {
                return;
            }

            _isFading = true;
            StartCoroutine(FadeOutRoutine(Mathf.Max(duration, 0.01f)));
        }

        private IEnumerator FadeOutRoutine(float duration)
        {
            var startColor = _spriteRenderer ? _spriteRenderer.color : Color.white;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (_spriteRenderer)
                {
                    var alpha = Mathf.Lerp(startColor.a, 0f, elapsed / duration);
                    _spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                }

                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}