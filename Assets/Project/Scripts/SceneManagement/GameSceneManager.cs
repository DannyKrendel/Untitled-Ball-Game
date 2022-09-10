using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.SceneManagement
{
    public class GameSceneManager : MonoBehaviour
    {
        [SerializeField] private SceneGroupCollection _sceneGroupCollection;
    
        private const string LevelsPath = "Assets/Project/Scenes/Levels/";
    
        private List<int> _levelBuildIndices;

        private void Awake()
        {
            _levelBuildIndices = GetLevelBuildIndices();
        }

        public IEnumerator LoadMainMenu()
        {
            var scenesToUnload = GetCurrentScenes();
            yield return LoadScenesFromGroup("Core");
            yield return LoadScenesFromGroup("Main Menu");
            yield return UnloadScenes(scenesToUnload);
        }

        public IEnumerator LoadLevel(int index)
        {
            if (index < 0 || index > _levelBuildIndices.Count)
            {
                Debug.LogWarning($"Couldn't load level with index {index}.");
                yield break;
            }
        
            var levelBuildIndex = _levelBuildIndices[index];
            var scene = SceneManager.GetSceneByBuildIndex(levelBuildIndex);

            if (scene.isLoaded)
            {
                Debug.LogWarning($"Level '{scene.name}' is already loaded.");
                yield break;
            }
        
            var scenesToUnload = GetCurrentScenes();
        
            yield return LoadScenesFromGroup("Level");

            var op = SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
            op.completed += o =>
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        
            yield return new WaitUntil(() => op.isDone);
        
            yield return UnloadScenes(scenesToUnload);
        }

        private IEnumerable<Scene> GetCurrentScenes()
        {
            var scenes = new List<Scene>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                scenes.Add(scene);
            }

            return scenes;
        }
    
        private IEnumerator UnloadScenes(IEnumerable<Scene> scenes)
        {
            var operations = new List<AsyncOperation>();

            foreach (var scene in scenes)
            {
                if (_sceneGroupCollection.IsSceneInGroup("Core", scene)) continue;
                operations.Add(SceneManager.UnloadSceneAsync(scene));
            }
        
            if (operations.Count == 0) yield break;
        
            yield return new WaitUntil(() => operations.All(x => x.isDone));
        }

        private IEnumerator LoadScenesFromGroup(string groupName)
        {
            var sceneGroup = _sceneGroupCollection.Find(groupName);

            var loadedScenes = new List<Scene>();
        
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i));
            }
        
            var operations = new List<AsyncOperation>();
        
            foreach (var scene in sceneGroup.Scenes)
            {
                if (loadedScenes.Any(s => s.buildIndex == scene.BuildIndex)) continue;
                operations.Add(SceneManager.LoadSceneAsync(scene.ScenePath, LoadSceneMode.Additive));
            }

            if (operations.Count == 0) yield break;
        
            yield return new WaitUntil(() => operations.All(x => x.isDone));
        }

        private List<int> GetLevelBuildIndices()
        {
            var sceneIndices = new List<int>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                if (scenePath.StartsWith(LevelsPath))
                    sceneIndices.Add(i);
            }

            return sceneIndices;
        }
    }
}