using UnityEngine;

public static class GameContentProvider
{
    private static PortalDatabase portalDatabase;

    public static PortalDatabase Portals
    {
        get
        {
            if (portalDatabase != null)
                return portalDatabase;

            PortalDatabase asset = Resources.Load<PortalDatabase>("PortalDatabase");
            portalDatabase = asset != null
                ? asset
                : DefaultPortalDatabaseFactory.CreateRuntimeDatabase();

            portalDatabase = DefaultPortalDatabaseFactory.EnsureDefaults(portalDatabase);
            return portalDatabase;
        }
    }

    public static void ResetCache()
    {
        portalDatabase = null;
    }
}
