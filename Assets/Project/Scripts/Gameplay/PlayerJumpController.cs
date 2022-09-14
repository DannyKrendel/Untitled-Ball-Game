using System;
using UnityEngine;

namespace Project.Gameplay
{
    public class PlayerJumpController : MonoBehaviour
    {
        [SerializeField] private Player _player;
    
        public event Action CalculationStarted;
        public event Action CalculationEnded;
    
        public Vector2 CurrentDirection { get; private set; }
        public float CurrentPower { get; private set; }

        private bool _calculationStarted;

        public void Set(Vector2 direction, float power)
        {
            if (GameState.IsPaused || !_player.IsGrounded() || _player.Velocity > 0) return;
        
            if (!_calculationStarted) OnCalculationStarted();
        
            CurrentDirection = direction;
            CurrentPower = power;
        }
    
        public void Jump()
        {
            if (CurrentDirection == Vector2.zero || CurrentPower == 0) return;
        
            _player.Jump(CurrentDirection.normalized, CurrentPower);
            OnCalculationEnded();
        }

        private void OnCalculationStarted()
        {
            _calculationStarted = true;
            CalculationStarted?.Invoke();
        }

        private void OnCalculationEnded()
        {
            CurrentDirection = Vector2.zero;
            CurrentPower = 0;
            _calculationStarted = false;
            CalculationEnded?.Invoke();
        }
    }
}
