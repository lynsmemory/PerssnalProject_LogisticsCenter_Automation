using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// One-off: removes the warehouse asset's training/demo scripts (Assets/Scripts/DistributionCenter, ImmersiveTraining)
// from the main scene before the script folders are moved out of the project.
// Roots that exist only for those scripts are deleted whole; elsewhere only the components are removed.
// Batch: Unity -batchmode -projectPath <project> -executeMethod AssetScriptCleanup.DryRun|Apply [-cleanupOut <file>]
public static class AssetScriptCleanup
{
    const string ScenePath = "Assets/Scenes/LogisticCenter_Automation.unity";
    static readonly string[] Folders = { "Assets/Scripts/DistributionCenter/", "Assets/Scripts/ImmersiveTraining/" };
    static readonly string[] Roots = { "DistributionCenterScripts", "ImmersiveTraining" };

    public static void DryRun() => Run(false);
    public static void Apply() => Run(true);

    static void Run(bool apply)
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var log = new StringBuilder();
        foreach (var root in scene.GetRootGameObjects())
            if (Array.IndexOf(Roots, root.name) >= 0)
            {
                log.AppendLine($"ROOT {root.name}:");
                foreach (var t in root.GetComponentsInChildren<Transform>(true))
                    log.AppendLine($"  {PathOf(t)} [{string.Join(", ", Names(t.gameObject))}]");
            }
        var targets = new List<MonoBehaviour>();
        foreach (var root in scene.GetRootGameObjects())
            foreach (var mb in root.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (mb == null) continue;
                string path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(mb));
                if (!Array.Exists(Folders, f => path.StartsWith(f))) continue;
                bool inRoot = Array.IndexOf(Roots, mb.transform.root.name) >= 0;
                log.AppendLine($"{(inRoot ? "in root " : "OUTSIDE ")}{PathOf(mb.transform)} : {Path.GetFileName(path)}{(PrefabUtility.IsPartOfPrefabInstance(mb) ? " (prefab instance)" : "")}");
                if (!inRoot) targets.Add(mb);
            }
        if (apply)
        {
            foreach (var mb in targets) UnityEngine.Object.DestroyImmediate(mb, true);
            foreach (var root in scene.GetRootGameObjects())
                if (Array.IndexOf(Roots, root.name) >= 0) UnityEngine.Object.DestroyImmediate(root);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            log.AppendLine("applied and saved");
        }
        File.WriteAllText(Arg("-cleanupOut") ?? "asset_script_cleanup.txt", log.ToString());
        if (Application.isBatchMode) EditorApplication.Exit(0);
    }

    static IEnumerable<string> Names(GameObject go)
    {
        foreach (var c in go.GetComponents<Component>()) yield return c == null ? "MISSING" : c.GetType().Name;
    }

    static string PathOf(Transform t) => t.parent == null ? t.name : PathOf(t.parent) + "/" + t.name;

    static string Arg(string key)
    {
        var a = Environment.GetCommandLineArgs();
        for (int i = 0; i < a.Length - 1; i++) if (a[i] == key) return a[i + 1];
        return null;
    }
}
