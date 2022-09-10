using System.Linq;
using UnityEditor;

#if UNITY_EDITOR
namespace Project.SceneManagement
{
    public static class EditorSceneHelper
    {
        private static string[] _scenePaths;
        public static string[] ScenePaths
        {
            get { return _scenePaths ??= EditorBuildSettings.scenes.Select(s => s.path).ToArray(); }
        }

        public static int GetBuildIndexFromPath(string path)
        {
            var foundScene = ScenePaths.Select((p, i) => (p, i)).FirstOrDefault(tuple => tuple.p == path);
            return foundScene == default ? -1 : foundScene.i;
        }

        public static string GetPathFromBuildIndex(int buildIndex)
        {
            var foundScene = ScenePaths.Select((p, i) => (p, i)).FirstOrDefault(tuple => tuple.i == buildIndex);
            return foundScene == default ? null : foundScene.p;
        }
        

        public static SceneAsset GetAssetFromPath(string path)
        {
            return string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
        }

        public static string GetPathFromAsset(SceneAsset sceneAsset)
        {
            return sceneAsset == null ? string.Empty : AssetDatabase.GetAssetPath(sceneAsset);
        }
    }
}
#endif