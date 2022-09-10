using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class OptionsScreen : ScreenBase
    {
        [SerializeReference] private Button _closeButton;

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(Hide);
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveAllListeners();
        }
    }
}
