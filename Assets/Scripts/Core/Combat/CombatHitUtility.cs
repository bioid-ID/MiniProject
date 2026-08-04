using UnityEngine;

public static class CombatHitUtility
{
    public static DamageInfo BuildPlayerAttack(
        GameObject attacker,
        float damage,
        Vector2 direction,
        AttackMethod method,
        DamageType damageType = DamageType.Physical)
    {
        var bal = GameBalance.Config; // BALANCE
        PlayerStat stat = PlayerStat.Instance;
        bool isCritical = false;
        float critMultiplier = 1.5f;
        bool isProjectile = method == AttackMethod.Projectile || method == AttackMethod.Skill;
        float knockback = isProjectile ? bal.projectileKnockback : bal.meleeKnockback;
        float stun = isProjectile ? bal.projectileStun : bal.meleeStun;

        if (stat != null)
        {
            isCritical = Random.value * 100f < stat.CriticalChance;
            critMultiplier = StatCalculator.CalculateCriticalDamage(stat);

            float weaponKnock = 0f;
            if (stat.weaponSlot != null)
                weaponKnock = stat.weaponSlot.knockBack;

            if (weaponKnock > 0f)
                knockback = weaponKnock;
            else if (!isProjectile)
                knockback = bal.meleeKnockback + stat.AttackDamage * 0.02f;

            if (isCritical)
            {
                knockback *= bal.critKnockbackMult;
                stun *= bal.critStunMult;
            }
        }

        if (direction.sqrMagnitude < 0.001f)
            direction = Vector2.right;

        return new DamageInfo(
            attacker,
            damage,
            damageType,
            TeamType.Player,
            isCritical,
            critMultiplier,
            0f,
            knockback,
            stun,
            direction.normalized,
            method);
    }

    public static void ApplyOnHitEffects(Enemy enemy, DamageInfo damageInfo, float dealtDamage)
    {
        if (enemy == null || damageInfo.Team != TeamType.Player)
            return;

        ApplyHitReaction(enemy, damageInfo);
        ApplySteal(dealtDamage);
        DungeonManager.Instance?.RunStats.LogDamageDealt(
            dealtDamage,
            damageInfo.AttackMethod,
            damageInfo.DamageType,
            damageInfo.IsCritical);
    }

    private static void ApplyHitReaction(Enemy enemy, DamageInfo damageInfo)
    {
        Vector2 direction = damageInfo.HitDirection;
        if (direction.sqrMagnitude < 0.001f && damageInfo.Attacker != null)
            direction = ((Vector2)enemy.transform.position - (Vector2)damageInfo.Attacker.transform.position).normalized;

        enemy.ApplyHitReaction(direction, damageInfo.KnockbackForce, damageInfo.StunDuration);
    }

    private static void ApplySteal(float dealtDamage)
    {
        if (dealtDamage <= 0f || PlayerStat.Instance == null)
            return;

        float lifeSteal = PlayerStat.Instance.LifeSteal;
        float manaSteal = PlayerStat.Instance.ManaSteal;

        if (lifeSteal > 0f)
        {
            float heal = dealtDamage * lifeSteal;
            PlayerHealth health = PlayerManager.Instance != null
                ? PlayerManager.Instance.Health
                : Object.FindFirstObjectByType<PlayerHealth>();
            health?.Heal(heal, HealSource.LifeSteal);
        }

        if (manaSteal > 0f)
            PlayerStat.Instance.RestoreMp(dealtDamage * manaSteal, ManaSource.ManaSteal);
    }
}
