using UnityEngine;

namespace Project.Gameplay
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _maxJumpHeight = 3;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField, Range(0, 1)] private float _groundCheckDistance = .1f;

        public float Velocity => _rigidbody2D.velocity.magnitude;
        public float MaxJumpHeight => _maxJumpHeight;

        private Vector2 _spawnPosition;
        private bool _lastGrounded;
        private Rigidbody2D _rigidbody2D;
        private Collider2D _collider2D;
        private Damageable _damageable;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();
            _damageable = GetComponent<Damageable>();
            _damageable.Died += Respawn;
        }

        private void Update()
        {
            var isGrounded = IsGrounded();
        
            if (isGrounded && !_lastGrounded && _rigidbody2D.velocity.x != 0)
            {
                _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
            }
        
            _lastGrounded = isGrounded;
        }

        public void Jump(Vector2 direction, float power)
        {
            if (Vector2.Dot(transform.up, direction) <= 0) return;
            var height = _maxJumpHeight * power;
            var force = direction * Mathf.Sqrt(2 * Physics2D.gravity.magnitude * _rigidbody2D.gravityScale * height);
            _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        }
    
        public bool IsGrounded()
        {
            var bounds = _collider2D.bounds;
            bounds.size = new Vector3(bounds.size.x, bounds.size.y / 2);
            bounds.center += Vector3.down * (bounds.size.y / 2);
        
            // Debug.DrawRay(bounds.center + new Vector3(bounds.extents.x, 0), -transform.up * (bounds.extents.y + _groundCheckDistance), Color.red);
            // Debug.DrawRay(bounds.center - new Vector3(bounds.extents.x, 0), -transform.up * (bounds.extents.y + _groundCheckDistance), Color.red);
            // Debug.DrawRay(bounds.center - new Vector3(bounds.extents.x, bounds.extents.y + _groundCheckDistance), transform.right * bounds.size.x, Color.red);
        
            return Physics2D.BoxCast(bounds.center, bounds.size, 0, -transform.up, _groundCheckDistance, _groundMask);
        }

        private void Respawn()
        {
            _rigidbody2D.velocity = Vector2.zero;
            _damageable.RestoreHealth();
            transform.position = _spawnPosition;
        }

        public void SetSpawnPosition(Vector2 position)
        {
            _spawnPosition = position;
        }
    }
}
