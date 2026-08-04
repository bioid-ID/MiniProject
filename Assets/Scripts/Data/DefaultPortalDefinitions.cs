using UnityEngine;

public static class DefaultPortalDefinitions
{
    private static PortalData mainDungeonPortal;
    private static PortalData roguelikeDungeonPortal;
    private static PortalData returnHubPortal;

    public static PortalData MainDungeon => GetOrCreateMainDungeon();
    public static PortalData RoguelikeDungeon => GetOrCreateRoguelikeDungeon();
    public static PortalData ReturnHub => GetOrCreateReturnHub();

    // Back-compat
    public static PortalData TestDungeon => RoguelikeDungeon;

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

    private static PortalData GetOrCreateRoguelikeDungeon()
    {
        if (roguelikeDungeonPortal != null)
            return roguelikeDungeonPortal;

        roguelikeDungeonPortal = ScriptableObject.CreateInstance<PortalData>();
        roguelikeDungeonPortal.portalId = "portal_roguelike_dungeon";
        roguelikeDungeonPortal.displayName = "Roguelike Dungeon";
        roguelikeDungeonPortal.flow = PortalFlow.EnterDungeon;
        roguelikeDungeonPortal.targetSceneName = GameSceneNames.RoguelikeDungeon;
        roguelikeDungeonPortal.resetDungeonRun = true;
        roguelikeDungeonPortal.placeholderColor = new Color(0.85f, 0.55f, 0.25f);
        return roguelikeDungeonPortal;
    }

    private static PortalData GetOrCreateReturnHub()
    {
        if (returnHubPortal != null)
            return returnHubPortal;

        returnHubPortal = ScriptableObject.CreateInstance<PortalData>();
        returnHubPortal.portalId = "portal_return_hub";
        returnHubPortal.displayName = "Lobby";
        returnHubPortal.flow = PortalFlow.ReturnToHub;
        returnHubPortal.targetSceneName = GameSceneNames.Hub;
        returnHubPortal.resetDungeonRun = false;
        returnHubPortal.placeholderColor = new Color(0.45f, 0.95f, 0.55f);
        return returnHubPortal;
    }
}
