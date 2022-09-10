using UnityEngine;

namespace Project.Gameplay
{
    public class Spike : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Damageable damageable))
            {
                damageable.Kill();
            }
        }
    }
}
