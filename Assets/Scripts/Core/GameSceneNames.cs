using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public static class GameSceneNames
{
    public const string Title = "TitleScene";

    // Canonical names (rename scenes in Unity Project window to these)
    public const string Hub = "Lobby";
    public const string MainDungeon = "MainDungeon";
    public const string RoguelikeDungeon = "RoguelikeDungeon";

    // Legacy names kept so current Build Settings keep working until you rename files.
    public const string HubLegacy = "SampleScene";
    public const string Nexus = "Nexus";
    public const string MainDungeonLegacy = "PortalDungeon";
    public const string RoguelikeDungeonLegacy = "PortalDungeon 1";
    public const string RoguelikeDungeonLegacyCompact = "PortalDungeon1";

    // Back-compat aliases used by older code.
    public const string TestDungeon = RoguelikeDungeon;
    public const string TestDungeonLegacy = RoguelikeDungeonLegacyCompact;

    private static readonly Dictionary<string, string[]> Aliases = new Dictionary<string, string[]>
    {
        { Hub, new[] { HubLegacy, Nexus } },
        { HubLegacy, new[] { Hub, Nexus } },
        { Nexus, new[] { Hub, HubLegacy } },
        { MainDungeon, new[] { MainDungeonLegacy } },
        { MainDungeonLegacy, new[] { MainDungeon } },
        { RoguelikeDungeon, new[] { RoguelikeDungeonLegacy, RoguelikeDungeonLegacyCompact } },
        { RoguelikeDungeonLegacy, new[] { RoguelikeDungeon, RoguelikeDungeonLegacyCompact } },
        { RoguelikeDungeonLegacyCompact, new[] { RoguelikeDungeon, RoguelikeDungeonLegacy } }
    };

    public static string Resolve(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return sceneName;

        if (IsInBuild(sceneName))
            return sceneName;

        if (Aliases.TryGetValue(sceneName, out string[] aliases))
        {
            foreach (string alias in aliases)
            {
                if (IsInBuild(alias))
                    return alias;
            }
        }

        return sceneName;
    }

    public static bool IsTitleScene(string sceneName) => sceneName == Title;

    public static bool IsHubScene(string sceneName)
    {
        return sceneName == Hub
            || sceneName == HubLegacy
            || sceneName == Nexus;
    }

    public static bool IsHubScene() => IsHubScene(SceneManager.GetActiveScene().name);

    public static bool IsMainDungeonScene(string sceneName)
    {
        return sceneName == MainDungeon || sceneName == MainDungeonLegacy;
    }

    public static bool IsRoguelikeDungeonScene(string sceneName)
    {
        return sceneName == RoguelikeDungeon
            || sceneName == RoguelikeDungeonLegacy
            || sceneName == RoguelikeDungeonLegacyCompact;
    }

    public static bool IsDungeonScene(string sceneName)
    {
        return IsMainDungeonScene(sceneName) || IsRoguelikeDungeonScene(sceneName);
    }

    public static bool IsDungeonScene() => IsDungeonScene(SceneManager.GetActiveScene().name);

    public static DungeonRunMode GetRunModeForScene(string sceneName)
    {
        if (IsRoguelikeDungeonScene(sceneName))
            return DungeonRunMode.Roguelike;

        if (IsMainDungeonScene(sceneName))
            return DungeonRunMode.Standard;

        return DungeonRunMode.None;
    }

    private static bool IsInBuild(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            if (string.IsNullOrWhiteSpace(path) || !path.EndsWith(".unity"))
                continue;

            if (Path.GetFileNameWithoutExtension(path) == sceneName)
                return true;
        }

        return false;
    }
}

public enum DungeonRunMode
{
    None,
    Standard,   // keep character progress
    Roguelike   // reset to base stats; keep gold only
}
