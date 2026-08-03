using UnityEngine;

[CreateAssetMenu(fileName = "PortalDatabase", menuName = "MiniProject/Portal Database")]
public class PortalDatabase : ScriptableObject
{
    public PortalSpawnDefinition[] hubPortals;
    public PortalSpawnDefinition[] dungeonPortals;
}
