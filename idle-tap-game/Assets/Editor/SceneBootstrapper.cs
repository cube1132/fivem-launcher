#if UNITY_EDITOR
using System.IO;
using IdleTapGame.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace IdleTapGame.EditorTools
{
    /// <summary>
    /// One-click scene setup so the project "just runs" after a fresh open.
    /// (Fallback: create any empty scene and add a GameBootstrap component.)
    /// </summary>
    public static class SceneBootstrapper
    {
        [MenuItem("IdleTapGame/Create Main Scene")]
        public static void CreateMainScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var go = new GameObject("GameBootstrap");
            go.AddComponent<GameBootstrap>();

            const string dir = "Assets/Scenes";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            const string path = dir + "/Main.unity";
            EditorSceneManager.SaveScene(scene, path);
            AssetDatabase.Refresh();

            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(path, true) };

            Debug.Log("[IdleTapGame] Created " + path + " and set it as the build scene. Press Play.");
        }
    }
}
#endif
