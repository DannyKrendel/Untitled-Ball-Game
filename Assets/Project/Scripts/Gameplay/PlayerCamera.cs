using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Project.Gameplay
{
    public class PlayerCamera : MonoBehaviour
    {
        private CinemachineVirtualCamera _camera;
        private Player _player;
    
        private void Awake()
        {
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() =>
            {
                _player = FindObjectOfType<Player>();
                return _player != null;
            });
        
            _camera.Follow = _player.transform;
        }
    }
}
