using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _optionsButton;
        [SerializeField] private Button _quitButton;
    
        [Header("Screens")]
        [SerializeField] private LevelScreen _levelScreen;
        [SerializeField] private OptionsScreen _optionsScreen;

        private void Awake()
        {
            _levelScreen.gameObject.SetActive(false);
            _optionsScreen.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _playButton.onClick.AddListener(OnPlay);
            _optionsButton.onClick.AddListener(OnOptions);
            _quitButton.onClick.AddListener(OnQuit);

            _levelScreen.CreateLevelButtons(new[] {"1", "2", "3"}); // placeholder
        }

        private void OnPlay()
        {
            _levelScreen.Show();
        }

        private void OnOptions()
        {
            _optionsScreen.Show();
        }
    
        private void OnQuit() => Application.Quit();
    }
}
