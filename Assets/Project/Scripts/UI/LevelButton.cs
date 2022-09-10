using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _button;

        public string Text
        {
            get => _text.text;
            set => _text.text = value;
        }

        public void AddListener(Action action)
        {
            if (action != null)
                _button.onClick.AddListener(action.Invoke);
        }
    }
}
