using System.Collections.Generic;
using System.Linq;
using PolyternityStuff.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class LevelScreen : ScreenBase
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private GameObject _levelButtonPrefab;
        [SerializeField] private Transform _parent;
    
        private List<LevelButton> _buttons;

        public void CreateLevelButtons(IEnumerable<string> levels)
        {
            _buttons = new List<LevelButton>();
            for (int i = 0; i < levels.Count(); i++)
            {
                var level = levels.ElementAt(i);
                var button = Instantiate(_levelButtonPrefab, _parent);
                var levelButton = button.GetComponent<LevelButton>();
                levelButton.Text = level;
                var index = i;
                levelButton.AddListener(() => OnLevelSelected(index));
                _buttons.Add(levelButton);
            }
        }

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(Hide);
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(Hide);
        }

        private void OnLevelSelected(int index)
        {
            StartCoroutine(FindObjectOfType<GameSceneManager>().LoadLevel(index));
        }
    }
}
