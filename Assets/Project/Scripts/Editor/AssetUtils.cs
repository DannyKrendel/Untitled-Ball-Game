using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Project.Editor
{
    public static class AssetUtils
    {
        public static T GetScriptableObject<T>(string name)
            where T : ScriptableObject
        {
            var paths = AssetDatabase.FindAssets($"t:{typeof(T).Name}").Select(AssetDatabase.GUIDToAssetPath);
            var assetPath = paths.FirstOrDefault(p => p.EndsWith(name + ".asset"));
            if (string.IsNullOrEmpty(assetPath)) return null;
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        private static string GetPathFromTypeAndName(string type, string name)
        {
            var assets = AssetDatabase.FindAssets($"t:{type} {name}");

            if (assets.Length == 0)
            {
                Debug.LogWarning($"Asset {name} of type {type} was not found.");
                return string.Empty;
            }
            if (assets.Length > 1)
            {
                Debug.LogWarning($"Multiple assets with name {name} of type {type} were found.");
                return string.Empty;
            }
            
            return AssetDatabase.GUIDToAssetPath(assets[0]);
        }
    }
}
