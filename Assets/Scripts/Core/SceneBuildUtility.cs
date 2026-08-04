using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public static class SceneBuildUtility
{
    public static string ResolveSceneName(string sceneName)
    {
        return GameSceneNames.Resolve(sceneName);
    }

    public static bool IsSceneInBuild(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return false;

        string resolved = ResolveSceneName(sceneName);
        return IsSceneInBuildDirect(resolved);
    }

    private static bool IsSceneInBuildDirect(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return false;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            if (string.IsNullOrWhiteSpace(scenePath) || !scenePath.EndsWith(".unity"))
                continue;

            string buildSceneName = Path.GetFileNameWithoutExtension(scenePath);
            if (buildSceneName == sceneName)
                return true;
        }

        return false;
    }

    public static string GetMissingSceneHelpMessage(string sceneName)
    {
        string resolved = ResolveSceneName(sceneName);

        return
            $"Scene '{sceneName}' is not in Build Settings.\n" +
            (resolved != sceneName ? $"Resolved alias: '{resolved}'\n" : string.Empty) +
            "Unity: Project 창에서 씬 이름을 Lobby / MainDungeon / RoguelikeDungeon 으로 바꾼 뒤\n" +
            "File > Build Settings > Scenes In Build 를 확인하세요.\n" +
            $"Expected path example: Assets/Scenes/{resolved}.unity";
    }
}
