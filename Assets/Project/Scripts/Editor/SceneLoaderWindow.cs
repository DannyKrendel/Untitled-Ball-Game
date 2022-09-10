using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Project.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Editor
{
    [InitializeOnLoad]
    public class SceneLoaderWindow : EditorWindow
    {
        private const string LevelsPath = "Assets/Project/Scenes/Levels/";
        private static SceneGroupCollection _sceneGroupCollection;

        private static List<string> _scenePathsToLoad = new List<string>();
    
        static SceneLoaderWindow()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
    
        private void OnEnable()
        {
            _sceneGroupCollection = AssetUtils.GetScriptableObject<SceneGroupCollection>("SceneGroupCollection");
        }

        [MenuItem("Window/Scene Loader")]
        private static void Init()
        {
            GetWindow<SceneLoaderWindow>("Scene Loader");
        }

        private void OnGUI()
        {
            GUILayout.Label("Main Menu", EditorStyles.boldLabel);

            if (GUILayout.Button("Main Menu"))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    LoadMainMenu();
            }
        
            GUILayout.Label("Levels", EditorStyles.boldLabel);

            var levelScenes = GetLevelScenes();

            foreach (var levelScene in levelScenes)
            {
                if (GUILayout.Button(levelScene.Name))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        LoadLevel(levelScene.Path);
                }
            }
        }

        private void LoadMainMenu()
        {
            _scenePathsToLoad.AddRange(LoadScenePathsFromGroupNames("Main Menu", "Core"));
            EditorApplication.EnterPlaymode();
        }
    
        private void LoadLevel(string path)
        {
            _scenePathsToLoad.Add(path);
            _scenePathsToLoad.AddRange(LoadScenePathsFromGroupNames("Core", "Level"));
            EditorApplication.EnterPlaymode();
        }
    
        private IEnumerable<LevelScene> GetLevelScenes()
        {
            var scenes = new List<LevelScene>();

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scene = EditorBuildSettings.scenes[i];
                if (scene.path.StartsWith(LevelsPath))
                    scenes.Add(new LevelScene
                    {
                        BuildIndex = i, Name = Path.GetFileNameWithoutExtension(scene.path), Path = scene.path
                    });
            }

            return scenes;
        }
    
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                OnEnteredEditMode();
            }
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                SaveCurrentScenes();
                OnExitingEditMode();
            }
        }

        private static void OnEnteredEditMode()
        {
            _scenePathsToLoad = new List<string>();
            var scenes = EditorPrefs.GetString("Scenes").Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            EditorPrefs.DeleteKey("Scenes");
        
            int i = 0;
            foreach (var scene in scenes)
            {
                EditorSceneManager.OpenScene(scene, i == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive);
                i++;
            }
        }

        private static void SaveCurrentScenes()
        {
            var scenes = "";
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                scenes += SceneManager.GetSceneAt(i).path + ",";
            }
            EditorPrefs.SetString("Scenes", scenes);
        }

        private static void OnExitingEditMode()
        {
            for (var i = 0; i < _scenePathsToLoad.Count; i++)
            {
                EditorSceneManager.OpenScene(_scenePathsToLoad[i], i == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive);
            }
        }

        private static IEnumerable<string> LoadScenePathsFromGroupNames(params string[] sceneGroupNames)
        {
            return sceneGroupNames
                .SelectMany(groupName => _sceneGroupCollection.Find(groupName).Scenes.Select(s => s.ScenePath));
        }

        private struct LevelScene
        {
            public string Name;
            public string Path;
            public int BuildIndex;
        }
    }
}