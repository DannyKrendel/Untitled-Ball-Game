using UnityEngine;

namespace Project.SceneManagement
{
    public class Preload : MonoBehaviour
    {
        [SerializeField] private GameSceneManager _gameSceneManager;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            StartCoroutine(_gameSceneManager.LoadMainMenu());
        }
    }
}
