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
                portal = DefaultPortalDefinitions.RoguelikeDungeon,
                objectName = "Portal_RoguelikeDungeon",
                worldPosition = new Vector3(-2.5f, 0f, 0f)
            }
        };

        cachedDatabase.dungeonPortals = new[]
        {
            new PortalSpawnDefinition
            {
                portal = DefaultPortalDefinitions.ReturnHub,
                objectName = "Portal_ReturnHub",
                worldPosition = new Vector3(0f, 2.5f, 0f)
            }
        };

        return cachedDatabase;
    }

    public static PortalDatabase EnsureDefaults(PortalDatabase database)
    {
        PortalDatabase defaults = CreateRuntimeDatabase();
        if (database == null)
            return defaults;

        if (database.hubPortals == null || database.hubPortals.Length == 0)
            database.hubPortals = defaults.hubPortals;

        if (database.dungeonPortals == null || database.dungeonPortals.Length == 0)
            database.dungeonPortals = defaults.dungeonPortals;

        return database;
    }
}
