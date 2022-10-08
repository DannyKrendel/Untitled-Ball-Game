using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PolyternityStuff.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Gameplay
{
    public class GameSceneManager : MonoBehaviour
    {
        private const string LevelsPath = "Assets/Project/Scenes/Levels/";
    
        private List<int> _levelBuildIndices;

        private void Awake()
        {
            _levelBuildIndices = GetLevelBuildIndices();
        }

        public IEnumerator LoadMainMenu()
        {
            var scenesToUnload = GetCurrentScenes();
            yield return LoadSceneByName("Core");
            yield return LoadSceneByName("MainMenu");
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
        
            yield return LoadSceneByName("UI");

            var op = SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
            op.completed += o => SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        
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
                if (scene.name == "Core") continue;
                operations.Add(SceneManager.UnloadSceneAsync(scene));
            }
        
            if (operations.Count == 0) yield break;
        
            yield return new WaitUntil(() => operations.All(x => x.isDone));
        }

        private IEnumerator LoadSceneByName(string sceneName)
        {
            var loadedScenes = new List<Scene>();
        
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i));
            }

            var scenePaths = SceneUtils.GetScenePaths();
            var scenePath = "";
            var sceneBuildIndex = -1;

            for (int i = 0; i < scenePaths.Length; i++)
            {
                if (SceneUtils.GetSceneNameFromPath(scenePaths[i]) == sceneName)
                {
                    scenePath = scenePaths[i];
                    sceneBuildIndex = i;
                    break;
                }
            }
            
            if (loadedScenes.All(s => s.buildIndex != sceneBuildIndex))
            {
                var operation = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
                yield return new WaitUntil(() => operation.isDone);
            }
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