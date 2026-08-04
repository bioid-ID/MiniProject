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

    public static void EnsureReturnPortal(Vector3 worldPosition)
    {
        PortalData returnPortal = DefaultPortalDefinitions.ReturnHub;
        UpsertPortal(returnPortal, "Portal_ReturnHub", worldPosition);
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

            UpsertPortal(
                definition.portal,
                string.IsNullOrWhiteSpace(definition.objectName) ? definition.portal.portalId : definition.objectName,
                definition.worldPosition);
        }
    }

    private static void UpsertPortal(PortalData data, string objectName, Vector3 worldPosition)
    {
        PortalTrigger existing = FindPortal(data.portalId);
        if (existing != null)
        {
            existing.transform.position = worldPosition;
            existing.gameObject.SetActive(true);
            existing.ApplyData(data);
            return;
        }

        PortalFactory.CreateFromData(data, objectName, worldPosition);
    }

    private static PortalTrigger FindPortal(string portalId)
    {
        PortalTrigger[] portals = Object.FindObjectsByType<PortalTrigger>(FindObjectsSortMode.None);
        foreach (PortalTrigger portal in portals)
        {
            if (portal != null && portal.PortalId == portalId)
                return portal;
        }

        return null;
    }
}
