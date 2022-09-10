using UnityEngine;

namespace Project.Gameplay
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;

        private void Start()
        {
            var obj = Instantiate(_playerPrefab, transform.position, Quaternion.identity);
            var player = obj.GetComponent<Player>();
            player.SetSpawnPosition(transform.position);
        }
    }
}
