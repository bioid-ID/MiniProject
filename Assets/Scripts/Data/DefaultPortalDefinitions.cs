using UnityEngine;

public static class DefaultPortalDefinitions
{
    private static PortalData mainDungeonPortal;
    private static PortalData testDungeonPortal;
    private static PortalData returnHubPortal;

    public static PortalData MainDungeon => GetOrCreateMainDungeon();
    public static PortalData TestDungeon => GetOrCreateTestDungeon();
    public static PortalData ReturnHub => GetOrCreateReturnHub();

    private static PortalData GetOrCreateMainDungeon()
    {
        if (mainDungeonPortal != null)
            return mainDungeonPortal;

        mainDungeonPortal = ScriptableObject.CreateInstance<PortalData>();
        mainDungeonPortal.portalId = "portal_main_dungeon";
        mainDungeonPortal.displayName = "Main Dungeon";
        mainDungeonPortal.flow = PortalFlow.EnterDungeon;
        mainDungeonPortal.targetSceneName = GameSceneNames.MainDungeon;
        mainDungeonPortal.resetDungeonRun = true;
        mainDungeonPortal.placeholderColor = new Color(0.35f, 0.75f, 1f);
        return mainDungeonPortal;
    }

    private static PortalData GetOrCreateTestDungeon()
    {
        if (testDungeonPortal != null)
            return testDungeonPortal;

        testDungeonPortal = ScriptableObject.CreateInstance<PortalData>();
        testDungeonPortal.portalId = "portal_test_dungeon";
        testDungeonPortal.displayName = "Test Dungeon";
        testDungeonPortal.flow = PortalFlow.EnterDungeon;
        testDungeonPortal.targetSceneName = GameSceneNames.TestDungeon;
        testDungeonPortal.resetDungeonRun = true;
        testDungeonPortal.placeholderColor = new Color(0.85f, 0.55f, 0.25f);
        return testDungeonPortal;
    }

    private static PortalData GetOrCreateReturnHub()
    {
        if (returnHubPortal != null)
            return returnHubPortal;

        returnHubPortal = ScriptableObject.CreateInstance<PortalData>();
        returnHubPortal.portalId = "portal_return_hub";
        returnHubPortal.displayName = "Nexus";
        returnHubPortal.flow = PortalFlow.ReturnToHub;
        returnHubPortal.targetSceneName = GameSceneNames.Hub;
        returnHubPortal.resetDungeonRun = false;
        returnHubPortal.placeholderColor = new Color(0.45f, 0.95f, 0.55f);
        return returnHubPortal;
    }
}
