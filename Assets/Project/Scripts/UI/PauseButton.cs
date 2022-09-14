using Project.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class PauseButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private PauseScreen _pauseScreen;

        private void Awake()
        {
            _pauseScreen.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnPausePressed);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OnPausePressed()
        {
            if (GameState.IsPaused)
            {
                _pauseScreen.Hide();
                GameState.IsPaused = false;
            }
            else
            {
                _pauseScreen.Show();
                GameState.IsPaused = true;
            }
        }
    }
}
