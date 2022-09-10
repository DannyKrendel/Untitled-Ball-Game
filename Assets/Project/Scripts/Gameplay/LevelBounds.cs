using UnityEngine;

namespace Project.Gameplay
{
    public class LevelBounds : MonoBehaviour
    {
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                var damageable = player.GetComponent<Damageable>();
                if (damageable)
                {
                    damageable.Kill();
                }
            }
        }
    }
}
