using UnityEngine.SceneManagement;

public static class GameSceneNames
{
    public const string Title = "TitleScene";
    public const string Hub = "SampleScene";
    public const string Nexus = "Nexus";
    public const string MainDungeon = "PortalDungeon";
    public const string TestDungeon = "PortalDungeon1";

    public static bool IsTitleScene(string sceneName)
    {
        return sceneName == Title;
    }

    public static bool IsHubScene(string sceneName)
    {
        return sceneName == Hub || sceneName == Nexus;
    }

    public static bool IsHubScene()
    {
        return IsHubScene(SceneManager.GetActiveScene().name);
    }

    public static bool IsDungeonScene(string sceneName)
    {
        return sceneName == MainDungeon || sceneName == TestDungeon;
    }

    public static bool IsDungeonScene()
    {
        return IsDungeonScene(SceneManager.GetActiveScene().name);
    }
}
