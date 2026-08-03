using UnityEngine;

[CreateAssetMenu(fileName = "PortalData", menuName = "MiniProject/Portal Data")]
public class PortalData : ScriptableObject
{
    [Header("Identity")]
    public string portalId = "portal_main_dungeon";
    public string displayName = "Main Dungeon";

    [Header("Flow")]
    public PortalFlow flow = PortalFlow.EnterDungeon;
    public string targetSceneName = GameSceneNames.MainDungeon;
    public bool resetDungeonRun = true;

    [Header("Prototype Visual (replace with Prefab art later)")]
    public Color placeholderColor = new Color(0.35f, 0.75f, 1f);
}
