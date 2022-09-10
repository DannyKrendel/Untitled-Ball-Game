using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Gameplay;
using Project.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Project.UI
{
    public class JumpControl : MonoBehaviour
    {
        [SerializeField] private RectTransform _jumpZone;
        [SerializeField] private RectTransform _handle;
        [SerializeField, Range(0, 180)] private float _angleRange = 180;
    
        public event Action JumpCalculationStarted;
        public event Action JumpCalculationEnded;
    
        public Vector2 CurrentJumpDirection { get; private set; }
        public float CurrentJumpPower { get; private set; }
    
        private InputManager _inputManager;
        private Player _player;
        private bool _grabStarted;
        private Vector2 _defaultPosition;
        private float _maxGrabDistance;
        private float _halfAngleRange;
        private Vector2 _minDirection;
        private Vector2 _maxDirection;

        private void Awake()
        {
            _halfAngleRange = _angleRange / 2;
            _minDirection = Quaternion.Euler(0, 0, -_halfAngleRange) * Vector2.down;
            _maxDirection = Quaternion.Euler(0, 0, _halfAngleRange) * Vector2.down;
        }

        private void OnEnable()
        {
            _inputManager = FindObjectOfType<InputManager>();
            _inputManager.TouchPressStarted += OnFingerDown;
            _inputManager.TouchPressEnded += OnFingerUp;
            _inputManager.PlayerControlsEnabled = true;
        }

        private void OnDisable()
        {
            _inputManager.TouchPressStarted -= OnFingerDown;
            _inputManager.TouchPressEnded -= OnFingerUp;
            _inputManager.PlayerControlsEnabled = false;
        }

        private IEnumerator Start()
        {
            var rectTransform = GetComponent<RectTransform>();
            _maxGrabDistance = rectTransform.sizeDelta.x / 2;
            _defaultPosition = transform.position;

            yield return new WaitWhile(() =>
            {
                _player = FindObjectOfType<Player>();
                return _player == null;
            });
        }

        private void Update()
        {
            if (GameManager.IsPaused || !_grabStarted || !_inputManager.PlayerControlsEnabled)
                return;
        
            OnFingerMove();
        }

        private void OnFingerDown()
        {
            if (GameManager.IsPaused || _grabStarted || !_player.IsGrounded()) return;

            var screenPosition = _inputManager.TouchPosition;

            if (!IsPositionInsideRectTransform(screenPosition, _jumpZone)) return;

            transform.position = screenPosition;
            
            OnJumpCalculationStarted();
        }
    
        private void OnFingerMove()
        {
            var screenPosition = _inputManager.TouchPosition;

            CurrentJumpDirection = ClampDirection(Vector2.ClampMagnitude((Vector2)transform.position - screenPosition, _maxGrabDistance));
            CurrentJumpPower = CalculateJumpPower(CurrentJumpDirection.magnitude);
        
            _handle.anchoredPosition = -CurrentJumpDirection;
        }

        private void OnFingerUp()
        {
            if (GameManager.IsPaused || !_grabStarted) return;
        
            CurrentJumpPower = CalculateJumpPower(CurrentJumpDirection.magnitude);
        
            _player.Jump(CurrentJumpDirection.normalized, CurrentJumpPower);

            transform.position = _defaultPosition;
            
            OnJumpCalculationEnded();
        }
    
        private float CalculateJumpPower(float currentGrabDistance)
        {
            return Mathf.InverseLerp(0, _maxGrabDistance, currentGrabDistance);
        }

        private Vector2 ClampDirection(Vector2 direction)
        {
            var currentAngle = Vector2.SignedAngle(Vector2.up, direction);

            if (currentAngle < -_halfAngleRange)
                direction = Quaternion.Euler(0, 0, -currentAngle - _halfAngleRange) * direction;
            else if (currentAngle > _halfAngleRange)
                direction = Quaternion.Euler(0, 0, -currentAngle + _halfAngleRange) * direction;
        
            return direction;
        }

        private void OnJumpCalculationStarted()
        {
            _grabStarted = true;
            JumpCalculationStarted?.Invoke();
        }
    
        private void OnJumpCalculationEnded()
        {
            _handle.anchoredPosition = Vector3.zero;
            CurrentJumpDirection = Vector2.zero;
            CurrentJumpPower = 0;
            _grabStarted = false;
            JumpCalculationEnded?.Invoke();
        }
        
        private static bool IsPositionInsideRectTransform(Vector2 position, RectTransform rectTransform)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, position, null, out var localPoint);
            return rectTransform.rect.Contains(localPoint);
        }
    }
}
