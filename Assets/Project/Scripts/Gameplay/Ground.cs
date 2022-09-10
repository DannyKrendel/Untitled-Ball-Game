using UnityEngine;

namespace Project.Gameplay
{
    public class Ground : MonoBehaviour
    {
        [SerializeField] private GroundType _type;

        public GroundType Type => _type;

        public enum GroundType
        {
            Normal
        }
    }
}