using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Project.SceneManagement
{
    [CreateAssetMenu(menuName = "Scene Group Collection", fileName = "New scene group collection")]
    public class SceneGroupCollection : ScriptableObject
    {
        [SerializeField] private List<SceneGroup> _sceneGroups;

        public SceneGroup Find(string name)
        {
            return _sceneGroups.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }
    
        public bool IsSceneInGroup(string groupName, Scene scene) =>
            Find(groupName).Scenes.Any(s => s.BuildIndex == scene.buildIndex);
    }

    [Serializable]
    public class SceneGroup
    {
        public string Name;
        public bool IsPersistent;
        public List<SceneReference> Scenes;
    }
}