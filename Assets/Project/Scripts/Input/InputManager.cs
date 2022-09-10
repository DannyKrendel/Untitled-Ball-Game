using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Project.Input
{
    public class InputManager : MonoBehaviour
    {
        private Controls _controls;

        public Vector2 TouchPosition => _controls.Player.TouchPosition.ReadValue<Vector2>();
        public event Action TouchPressStarted;
        public event Action TouchPressEnded;

        public bool PlayerControlsEnabled
        {
            get => _controls.Player.enabled;
            set
            {
                if (value)
                    _controls.Player.Enable();
                else
                    _controls.Player.Disable();
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void EnableEnhancedTouchSupport()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnEnable()
        {
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }
    
        private void Awake()
        {
            _controls = new Controls();
            _controls.Player.TouchPress.started += ctx => TouchPressStarted?.Invoke();
            _controls.Player.TouchPress.canceled += ctx => TouchPressEnded?.Invoke();
        }
    }
}
