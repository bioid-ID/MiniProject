#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Menu: Tools > Portal Dungeon > Rename Scenes To Final Names
/// Renames SampleScene/PortalDungeon/PortalDungeon 1 to Lobby/MainDungeon/RoguelikeDungeon
/// and updates Build Settings paths.
/// </summary>
public static class SceneRenameMenu
{
    private static readonly (string from, string to)[] Renames =
    {
        ("Assets/Scenes/SampleScene.unity", "Assets/Scenes/Lobby.unity"),
        ("Assets/Scenes/PortalDungeon.unity", "Assets/Scenes/MainDungeon.unity"),
        ("Assets/Scenes/PortalDungeon 1.unity", "Assets/Scenes/RoguelikeDungeon.unity")
    };

    [MenuItem("Tools/Portal Dungeon/Rename Scenes To Final Names")]
    public static void RenameScenes()
    {
        AssetDatabase.SaveAssets();

        foreach (var pair in Renames)
        {
            if (!File.Exists(pair.from))
            {
                if (File.Exists(pair.to))
                    Debug.Log($"[SceneRename] Already renamed: {pair.to}");
                else
                    Debug.LogWarning($"[SceneRename] Missing: {pair.from}");
                continue;
            }

            string error = AssetDatabase.MoveAsset(pair.from, pair.to);
            if (string.IsNullOrEmpty(error))
                Debug.Log($"[SceneRename] {pair.from} -> {pair.to}");
            else
                Debug.LogError($"[SceneRename] Failed {pair.from}: {error}");
        }

        UpdateBuildSettings();
        AssetDatabase.Refresh();
        Debug.Log("[SceneRename] Done. Play from Lobby scene.");
    }

    private static void UpdateBuildSettings()
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();

        string[] preferred =
        {
            "Assets/Scenes/Lobby.unity",
            "Assets/Scenes/MainDungeon.unity",
            "Assets/Scenes/RoguelikeDungeon.unity"
        };

        foreach (string path in preferred)
        {
            if (File.Exists(path))
                scenes.Add(new EditorBuildSettingsScene(path, true));
        }

        // Keep any other enabled scenes that were already in build.
        foreach (EditorBuildSettingsScene existing in EditorBuildSettings.scenes)
        {
            if (!existing.enabled)
                continue;

            bool already = false;
            foreach (var s in scenes)
            {
                if (s.path == existing.path)
                {
                    already = true;
                    break;
                }
            }

            if (!already && File.Exists(existing.path))
                scenes.Add(existing);
        }

        EditorBuildSettings.scenes = scenes.ToArray();
        Debug.Log($"[SceneRename] Build Settings updated ({scenes.Count} scenes).");
    }
}
#endif
