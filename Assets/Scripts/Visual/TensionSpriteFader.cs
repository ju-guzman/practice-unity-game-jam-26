using Core;
using UnityEngine;

namespace Visual
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TensionSpriteFader : MonoBehaviour
    {
        [Header("Range")]
        
        [SerializeField] private float transitionRandomTime;
        
        [SerializeField] private float transitionStartValue = 0.5f;
        [SerializeField] private float transitionEndValue = 0.2f;

        [Header("Opacity")]
        [SerializeField, Range(0f, 1f)] private float opacityAtStart;
        [SerializeField, Range(0f, 1f)] private float opacityAtEnd = 1f;

        [Header("Smoothing")]
        [Tooltip("Higher = faster reaction. The sprite's alpha eases toward the target instead of snapping to it.")]
        [SerializeField] private float smoothing = 3f;

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (transitionRandomTime == 0) return;
            transitionStartValue = Mathf.Clamp01(Random.value + transitionRandomTime);
            transitionEndValue = transitionStartValue - transitionRandomTime;
        }

        private void Update()
        {
            if (!GameManager.Instance)
            {
                return;
            }

            var t = Mathf.InverseLerp(transitionStartValue, transitionEndValue, GameManager.Instance.Value);
            var targetOpacity = Mathf.Lerp(opacityAtStart, opacityAtEnd, t);

            var color = _spriteRenderer.color;
            color.a = Mathf.MoveTowards(color.a, targetOpacity, smoothing * Time.deltaTime);
            _spriteRenderer.color = color;
        }
    }
}
