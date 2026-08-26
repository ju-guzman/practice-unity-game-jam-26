using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;

namespace Interaction
{
    public class RandomItemFadeSystem : MonoBehaviour
    {
        [SerializeField] private float interval = 15f;
        [SerializeField] private float fadeDuration = 10f;

        private readonly HashSet<CollectableItem> _alreadySelected = new();

        private void Start()
        {
            StartCoroutine(RunLoop());
        }

        private IEnumerator RunLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);
                TrySelectAndFade();
            }
        }

        private void TrySelectAndFade()
        {
            if (!GameManager.Instance)
            {
                return;
            }

            var candidates = GameManager.Instance.RegisteredCollectables
                .Where(item => item && item.gameObject.activeInHierarchy && !_alreadySelected.Contains(item))
                .ToList();

            if (candidates.Count == 0)
            {
                return;
            }

            var chosen = candidates[Random.Range(0, candidates.Count)];
            _alreadySelected.Add(chosen);
            chosen.BeginFade(fadeDuration);
        }
    }
}
