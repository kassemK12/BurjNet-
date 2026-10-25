#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

[InitializeOnLoad]
public static class SteelHorizonSceneSetup
{
    static SteelHorizonSceneSetup()
    {
        EditorApplication.delayCall += EnsureScene;
    }
    static void EnsureScene()
    {
        const string path = "Assets/Scenes/SteelHorizon.unity";
        if (File.Exists(path)) return;
        Directory.CreateDirectory("Assets/Scenes");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var root = new GameObject("Steel Horizon Bootstrap");
        root.AddComponent<SteelHorizonBootstrap>();
        EditorSceneManager.SaveScene(scene, path);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(path, true) };
        Debug.Log("Steel Horizon starter scene created. Press Play to test the prototype.");
    }
}
#endif
