using UnityEngine;


public struct DamageInfo
{
    public GameObject Attacker;

    public float Damage;

    public DamageType DamageType;

    public TeamType Team;

    public bool IsCritical;

    public float CriticalMultiplier;

    public float IgnoreDefense;

    public DamageInfo(
        GameObject attacker,
        float damage,
        DamageType damageType,
        TeamType team,
        bool critical = false,
        float criticalMultiplier = 1.5f,
        float ignoreDefense = 0f)
    {
        Attacker = attacker;
        Damage = damage;
        DamageType = damageType;
        Team = team;
        IsCritical = critical;
        CriticalMultiplier = criticalMultiplier;
        IgnoreDefense = ignoreDefense;
    }
}