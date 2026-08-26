using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputs
{
    public class InputManager : InputActions.IPlayerActions
    {
        private static InputManager _instance;

        public static InputManager Instance => _instance ??= new InputManager();

        public event Action<Vector2> PointerMoved;
        public event Action Clicked;
        public event Action NextRoomRequested;
        public event Action PreviousRoomRequested;
        public event Action RestartRequested;

        private readonly InputActions _inputActions;

        private InputManager()
        {
            _inputActions = new InputActions();
            _inputActions.Player.SetCallbacks(this);
            _inputActions.Player.Enable();
        }

        public Vector2 GetPointerScreenPosition()
        {
            return _inputActions.Player.Point.ReadValue<Vector2>();
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            PointerMoved?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Clicked?.Invoke();
            }
        }

        public void OnNextRoom(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                NextRoomRequested?.Invoke();
            }
        }

        public void OnPreviousRoom(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PreviousRoomRequested?.Invoke();
            }
        }

        public void OnRestart(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                RestartRequested?.Invoke();
            }
        }
    }
}
