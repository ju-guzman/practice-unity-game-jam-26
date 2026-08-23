using System;
using Inputs;
using UnityEngine;
using Utils;

namespace Interaction
{
    public class InteractionCursor2D : MonoBehaviour
    {
        public event Action<IInteractable> InteractableHovered;
        public event Action InteractableUnhovered;

        private Camera _targetCamera;
        private IInteractable _current;

        private void Awake()
        {
            _targetCamera = Camera.main;
        }

        private void OnEnable()
        {
            InputManager.Instance.PointerMoved += OnPointerMoved;
            InputManager.Instance.Clicked += OnClicked;
        }

        private void OnDisable()
        {
            InputManager.Instance.PointerMoved -= OnPointerMoved;
            InputManager.Instance.Clicked -= OnClicked;
        }

        private void OnPointerMoved(Vector2 screenPosition)
        {
            var worldPosition = ScreenToWorld.FromMouse(_targetCamera, screenPosition);
            var hit = Physics2D.OverlapPoint(worldPosition, InteractionLayers.InteractableMask);
            var interactable = hit ? hit.GetComponent<IInteractable>() : null;

            if (interactable == _current)
            {
                return;
            }

            _current?.OnHoverExit();
            if (_current != null)
            {
                InteractableUnhovered?.Invoke();
            }

            _current = interactable;

            if (_current == null) return;
            _current.OnHoverEnter();
            InteractableHovered?.Invoke(_current);
        }

        private void OnClicked()
        {
            _current?.Interact();
        }
    }
}