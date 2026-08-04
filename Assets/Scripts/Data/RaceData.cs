using UnityEngine;

/// <summary>
/// Future: race / skin starter stats + growth.
/// Not wired yet — placeholder so you can create assets early.
/// </summary>
[CreateAssetMenu(fileName = "NewRaceData", menuName = "ScriptableObjects/Race Data")]
public class RaceData : ScriptableObject
{
    public string raceName;

    [Header("Starter attributes")]
    public int baseStr = 10;
    public int baseDex = 10;
    public int baseInt = 10;
    public int baseLuck = 10;

    [Header("Growth bonuses (optional later)")]
    public float hpGrowthBonus;
    public float mpGrowthBonus;
    public float atkGrowthBonus;
}
