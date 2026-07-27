using UnityEngine;

public static class DamageCalculator
{
    public static float CalculateDamage(
        DamageInfo info,
        Character target)
    {
        float damage = info.Damage;

        if (info.IsCritical)
            damage *= 1.5f;

        float defense = 0f;

        switch (info.DamageType)
        {
            case DamageType.Physical:
                defense = target.PhysicalDefense;
                break;

            case DamageType.Magic:
            case DamageType.Fire:
            case DamageType.Ice:
            case DamageType.Lightning:
                defense = target.MagicDefense;
                break;

            case DamageType.True:
                defense = 0;
                break;

            case DamageType.Poison:
            case DamageType.Bleed:
                defense *= 0.5f;
                break;
        }

        damage *= 100f / (100f + defense);

        return Mathf.Max(1f, damage);
    }
}