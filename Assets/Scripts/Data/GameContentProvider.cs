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

            portalDatabase = Resources.Load<PortalDatabase>("PortalDatabase");
            if (portalDatabase == null)
                portalDatabase = DefaultPortalDatabaseFactory.CreateRuntimeDatabase();

            return portalDatabase;
        }
    }
}
