using System.Linq;
using Project.Gameplay;
using Project.Utility;
using UnityEditor;
using UnityEngine;

namespace Project.Editor
{
    [InitializeOnLoad]
    public static class GroundSnapManager
    {
        private static Vector3 _startWorldPos;
        private static Vector3[] _startSelectedPositions;
        private static Bounds _currentSelectionBounds;
        private static bool _isDragging;

        static GroundSnapManager()
        {
            SceneView.duringSceneGui += OnSceneViewGUI;
        }
        
        private static void OnSceneViewGUI(SceneView sceneView)
        {
            if (Selection.count == 0) return;
            
            var groundSnapTransforms = Selection.transforms.Where(x => x.TryGetComponent(out GroundSnap _)).ToList();

            if (groundSnapTransforms.Count == 0) return;

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    _startSelectedPositions = new Vector3[groundSnapTransforms.Count];
                    for (var i = 0; i < groundSnapTransforms.Count; i++)
                    {
                        _startSelectedPositions[i] = groundSnapTransforms[i].position;
                        if (groundSnapTransforms[i].TryGetComponent(out SpriteRenderer spriteRenderer))
                            _currentSelectionBounds.Encapsulate(spriteRenderer.bounds);
                    }
                    _startWorldPos = GetWorldPosFromMousePos(sceneView, Event.current.mousePosition);
                    break;
                case EventType.MouseDrag:
                    _isDragging = true;
                    break;
                case EventType.MouseUp or EventType.Ignore:
                    _startSelectedPositions = null;
                    _startWorldPos = Vector3.zero;
                    _currentSelectionBounds = default;
                    _isDragging = false;
                    break;
            }

            if (_isDragging && Event.current.type == EventType.Layout)
            {
                var dragDelta = GetWorldPosFromMousePos(sceneView, Event.current.mousePosition) - _startWorldPos;

                for (var i = 0; i < groundSnapTransforms.Count; i++)
                {
                    var transform = groundSnapTransforms[i];
                    
                    var ground = transform.GetComponentInParent<Ground>();
                    if (!ground) continue;
                    
                    var dragPos = _startSelectedPositions[i] + dragDelta;
                    
                    var groundSprite = ground.GetComponent<SpriteRenderer>();
                    var snappedPos = groundSprite.bounds.ClosestPoint(dragPos);
                    
                    Debug.Log($"world: {dragPos}; snapped: {snappedPos}");

                    if (Vector2.Distance(dragPos, snappedPos) < 1f)
                        transform.position = snappedPos;
                }

                Physics2D.SyncTransforms();
            }
        }

        private static Vector3 GetWorldPosFromMousePos(SceneView sceneView, Vector2 mousePos)
        {
            mousePos.x *= EditorGUIUtility.pixelsPerPoint;
            mousePos.y = sceneView.camera.pixelHeight - mousePos.y * EditorGUIUtility.pixelsPerPoint;
            return sceneView.camera.ScreenToWorldPoint(mousePos);
        }
    }
}