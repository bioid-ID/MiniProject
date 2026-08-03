using UnityEngine;

public static class DefaultPortalDatabaseFactory
{
    private static PortalDatabase cachedDatabase;

    public static PortalDatabase CreateRuntimeDatabase()
    {
        if (cachedDatabase != null)
            return cachedDatabase;

        cachedDatabase = ScriptableObject.CreateInstance<PortalDatabase>();
        cachedDatabase.hubPortals = new[]
        {
            new PortalSpawnDefinition
            {
                portal = DefaultPortalDefinitions.MainDungeon,
                objectName = "Portal_MainDungeon",
                worldPosition = new Vector3(2.5f, 0f, 0f)
            },
            new PortalSpawnDefinition
            {
                portal = DefaultPortalDefinitions.TestDungeon,
                objectName = "Portal_TestDungeon",
                worldPosition = new Vector3(-2.5f, 0f, 0f)
            }
        };

        cachedDatabase.dungeonPortals = new[]
        {
            new PortalSpawnDefinition
            {
                portal = DefaultPortalDefinitions.ReturnHub,
                objectName = "Portal_ReturnHub",
                worldPosition = new Vector3(-2.5f, 0f, 0f)
            }
        };

        return cachedDatabase;
    }
}
