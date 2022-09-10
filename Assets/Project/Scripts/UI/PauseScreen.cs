using Project.Gameplay;
using Project.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class PauseScreen : ScreenBase
    {
        [SerializeField] private Button _mainMenuButton;

        private void OnEnable()
        {
            _mainMenuButton.onClick.AddListener(OnMainMenuButtonPressed);
        }
    
        private void OnDisable()
        {
            _mainMenuButton.onClick.RemoveAllListeners();
        }

        private void OnMainMenuButtonPressed()
        {
            GameManager.IsPaused = false;
            StartCoroutine(FindObjectOfType<GameSceneManager>().LoadMainMenu());
        }
    }
}
