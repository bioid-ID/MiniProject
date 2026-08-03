using UnityEngine;

public static class PortalSpawner
{
    public static void SpawnHubPortals(PortalDatabase database)
    {
        if (database == null)
            return;

        SpawnDefinitions(database.hubPortals, PortalFlow.EnterDungeon);
    }

    public static void SpawnDungeonPortals(PortalDatabase database)
    {
        if (database == null)
            return;

        SpawnDefinitions(database.dungeonPortals, PortalFlow.ReturnToHub);
    }

    private static void SpawnDefinitions(PortalSpawnDefinition[] definitions, PortalFlow expectedFlow)
    {
        if (definitions == null)
            return;

        foreach (PortalSpawnDefinition definition in definitions)
        {
            if (definition?.portal == null)
                continue;

            if (definition.portal.flow != expectedFlow)
                continue;

            string portalId = definition.portal.portalId;
            if (string.IsNullOrWhiteSpace(portalId))
                continue;

            if (FindExistingPortal(portalId))
                continue;

            PortalFactory.CreateFromData(
                definition.portal,
                string.IsNullOrWhiteSpace(definition.objectName) ? definition.portal.portalId : definition.objectName,
                definition.worldPosition);
        }
    }

    private static bool FindExistingPortal(string portalId)
    {
        PortalTrigger[] portals = Object.FindObjectsByType<PortalTrigger>(FindObjectsSortMode.None);
        foreach (PortalTrigger portal in portals)
        {
            if (portal != null && portal.PortalId == portalId)
                return true;
        }

        return false;
    }
}
