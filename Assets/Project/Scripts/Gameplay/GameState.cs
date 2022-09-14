using UnityEngine;

namespace Project.Gameplay
{
    public static class GameState
    {
        public static bool IsPaused
        {
            get => _isPaused;
            set
            {
                Time.timeScale = value ? 0 : 1;
                _isPaused = value;
            }
        }

        private static bool _isPaused;
    }
}
