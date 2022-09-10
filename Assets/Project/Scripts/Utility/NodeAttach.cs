using UnityEngine;
using UnityEngine.U2D;

namespace Project.Utility
{
    [ExecuteInEditMode]
    public class NodeAttach : MonoBehaviour
    {
        [SerializeField] private SpriteShapeController _spriteShapeController;
        [SerializeField] private int _index;
        [SerializeField] private bool _useNormals;
        [SerializeField] private bool _runtimeUpdate;
        [SerializeField, Header("Offset")] private float _yOffset;
        [SerializeField] private bool _localOffset;
    
        private Spline _spline;
        private int _lastSpritePointCount;
        private bool _lastUseNormals;
        private Vector3 _lastPosition;

        private void Awake()
        {
            _spline = _spriteShapeController.spline;
        }

        private void Update()
        {
            if (!Application.isPlaying || _runtimeUpdate)
            {
                _spline = _spriteShapeController.spline;
                if ((_spline.GetPointCount() != 0) && (_lastSpritePointCount != 0))
                {
                    _index = Mathf.Clamp(_index, 0, _spline.GetPointCount() - 1);
                    if (_spline.GetPointCount() != _lastSpritePointCount)
                    {
                        if (_spline.GetPosition(_index) != _lastPosition)
                        {
                            _index += _spline.GetPointCount() - _lastSpritePointCount;
                        }
                    }

                    if ((_index <= _spline.GetPointCount() - 1) && (_index >= 0))
                    {
                        if (_useNormals)
                        {
                            if (_spline.GetTangentMode(_index) != ShapeTangentMode.Linear)
                            {
                                Vector3 lt =
                                    Vector3.Normalize(_spline.GetLeftTangent(_index) - _spline.GetRightTangent(_index));
                                Vector3 rt =
                                    Vector3.Normalize(_spline.GetLeftTangent(_index) - _spline.GetRightTangent(_index));
                                float a = Angle(Vector3.left, lt);
                                float b = Angle(lt, rt);
                                float c = a + (b * 0.5f);
                                if (b > 0) c = (180 + c);
                                transform.rotation = Quaternion.Euler(0, 0, c);
                            }
                        }
                        else
                        {
                            transform.rotation = Quaternion.Euler(0, 0, 0);
                        }

                        Vector3 offsetVector;
                        if (_localOffset)
                        {
                            offsetVector = (Vector3) Rotate(Vector2.up, transform.localEulerAngles.z) * _yOffset;
                        }
                        else
                        {
                            offsetVector = Vector2.up * _yOffset;
                        }

                        transform.position = _spriteShapeController.transform.position + _spline.GetPosition(_index) +
                                             offsetVector;
                        _lastPosition = _spline.GetPosition(_index);
                    }
                }
            }

            _lastSpritePointCount = _spline.GetPointCount();
        }

        private float Angle(Vector3 a, Vector3 b)
        {
            float dot = Vector3.Dot(a, b);
            float det = (a.x * b.y) - (b.x * a.y);
            return Mathf.Atan2(det, dot) * Mathf.Rad2Deg;
        }

        private Vector2 Rotate(Vector2 v, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);
            float tx = v.x;
            float ty = v.y;
            return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
        }
    }
}