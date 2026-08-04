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
    public float KnockbackForce;
    public float StunDuration;
    public Vector2 HitDirection;
    public AttackMethod AttackMethod;

    public DamageInfo(
        GameObject attacker,
        float damage,
        DamageType damageType,
        TeamType team,
        bool critical = false,
        float criticalMultiplier = 1.5f,
        float ignoreDefense = 0f,
        float knockbackForce = 0f,
        float stunDuration = 0f,
        Vector2 hitDirection = default,
        AttackMethod attackMethod = AttackMethod.Other)
    {
        Attacker = attacker;
        Damage = damage;
        DamageType = damageType;
        Team = team;
        IsCritical = critical;
        CriticalMultiplier = criticalMultiplier;
        IgnoreDefense = ignoreDefense;
        KnockbackForce = knockbackForce;
        StunDuration = stunDuration;
        HitDirection = hitDirection;
        AttackMethod = attackMethod;
    }
}
