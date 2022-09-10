using System.Collections;
using Project.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class JumpIndicator : MonoBehaviour
    {
        [SerializeField] private Image _arrow;
        [SerializeField] private Image _arrowFill;
        [SerializeField] private Vector2 _offsetFromPlayer;
        [SerializeField] private float _offsetFromCenter;
        [SerializeField] private JumpControl _jumpControl;

        private Player _player;

        private void Awake()
        {
            _arrow.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _jumpControl.JumpCalculationEnded += OnJumpCalculationEnded;
        }
    
        private void OnDisable()
        {
            _jumpControl.JumpCalculationEnded -= OnJumpCalculationEnded;
        }

        private IEnumerator Start()
        {
            yield return new WaitWhile(() =>
            {
                _player = FindObjectOfType<Player>();
                return _player == null;
            });
        }

        private void OnValidate()
        {
            _arrow.transform.position = transform.position + _arrow.transform.up * _offsetFromCenter;
        }
    
        private void Update()
        {
            if (_jumpControl.CurrentJumpDirection == Vector2.zero) return;
        
            if (!_arrow.gameObject.activeSelf) _arrow.gameObject.SetActive(true);
            _arrowFill.fillAmount = _jumpControl.CurrentJumpPower;
        
            transform.position = _player.transform.position + (Vector3)_offsetFromPlayer;
            var angle = Vector2.SignedAngle(Vector2.up, _jumpControl.CurrentJumpDirection);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void OnJumpCalculationEnded()
        {
            _arrow.gameObject.SetActive(false);
            _arrowFill.fillAmount = 0;
        }
    }
}
