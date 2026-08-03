using System.IO;
using UnityEngine.SceneManagement;

public static class SceneBuildUtility
{
    public static bool IsSceneInBuild(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return false;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string buildSceneName = Path.GetFileNameWithoutExtension(scenePath);

            if (buildSceneName == sceneName)
                return true;
        }

        return false;
    }

    public static string GetMissingSceneHelpMessage(string sceneName)
    {
        return
            $"Scene '{sceneName}' is not in Build Settings.\n" +
            "Unity: File > Build Profiles > Scenes In Build 에 씬을 추가하세요.\n" +
            $"Expected path example: Assets/Scenes/{sceneName}.unity";
    }
}
