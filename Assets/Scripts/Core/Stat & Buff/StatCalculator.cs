using UnityEngine;

public static class StatCalculator
{
    #region HP / MP

    public static float CalculateMaxHp(PlayerStat stat)
    {
        float value =
            100f +
            stat.CurrentLevel * 25f +
            stat.TotalStr * 15f +
            stat.SumEquipmentFloat(item => item.bonusMaxHealth);

        value += stat.GetModifierValue(
            StatType.MaxHp,
            ModifierType.Flat);

        value *=
            1f +
            stat.GetModifierValue(
                StatType.MaxHp,
                ModifierType.Percent);

        return value;
    }

    public static float CalculateMaxMp(PlayerStat stat)
    {
        float value =
            50f +
            stat.CurrentLevel * 10f +
            stat.TotalInt * 20f +
            stat.SumEquipmentFloat(item => item.bonusMaxMana);

        value += stat.GetModifierValue(
            StatType.MaxMp,
            ModifierType.Flat);

        value *=
            1f +
            stat.GetModifierValue(
                StatType.MaxMp,
                ModifierType.Percent);

        return value;
    }

    #endregion

    #region Attack

    public static float CalculateAttackDamage(PlayerStat stat)
    {
        float value =
            stat.weaponSlot == null
            ? 10f
            : stat.weaponSlot.CalculateWeaponDamage(stat);

        value += stat.GetModifierValue(
            StatType.Attack,
            ModifierType.Flat);

        value *=
            1f +
            stat.GetModifierValue(
                StatType.Attack,
                ModifierType.Percent);

        return value;
    }

    public static float CalculateAttackSpeed(PlayerStat stat)
    {
        float value =
            stat.weaponSlot == null
                ? 1f
                : stat.weaponSlot.attackSpeed;

        value +=
            stat.SumEquipmentFloat(
                item => item.bonusAttackSpeed);

        value +=
            stat.GetModifierValue(
                StatType.AttackSpeed,
                ModifierType.Flat);

        value *=
            1f +
            stat.GetModifierValue(
                StatType.AttackSpeed,
                ModifierType.Percent);

        return Mathf.Max(0.1f, value);
    }

    public static float CalculateAttackRange(PlayerStat stat)
    {
        float value =
            stat.weaponSlot == null
                ? 2.5f
                : stat.weaponSlot.attackRange;

        value +=
            stat.SumEquipmentFloat(
                item => item.bonusRange);

        value +=
            stat.GetModifierValue(
                StatType.AttackRange,
                ModifierType.Flat);

        value *=
            1f +
            stat.GetModifierValue(
                StatType.AttackRange,
                ModifierType.Percent);

        return value;
    }

    #endregion

    #region Defense

    public static float CalculateDefense(PlayerStat stat)
    {
        float value =
            stat.TotalStr * 0.5f +
            stat.SumEquipmentFloat(
                item => item.bonusDefense);

        value +=
            stat.GetModifierValue(
                StatType.Defense,
                ModifierType.Flat);

        value *=
            1f +
            stat.GetModifierValue(
                StatType.Defense,
                ModifierType.Percent);

        return value;
    }

    #endregion

    #region Move

    public static float CalculateMoveSpeed(PlayerStat stat)
    {
        float value =
            5f +
            stat.TotalDex * 0.004f +
            stat.SumEquipmentFloat(
                item => item.bonusMoveSpeed);

        value +=
            stat.GetModifierValue(
                StatType.MoveSpeed,
                ModifierType.Flat);

        value *=
            1f +
            stat.GetModifierValue(
                StatType.MoveSpeed,
                ModifierType.Percent);

        return value;
    }

    #endregion

    #region Critical

    public static float CalculateCriticalChance(PlayerStat stat)
    {
        float value =
            5.5f +
            stat.TotalLuck * 0.5f +
            stat.SumEquipmentFloat(
                item => item.bonusCritChance);

        value +=
            stat.GetModifierValue(
                StatType.CriticalChance,
                ModifierType.Flat);

        value *=
            1f +
            stat.GetModifierValue(
                StatType.CriticalChance,
                ModifierType.Percent);

        return value;
    }

    public static float CalculateCriticalDamage(PlayerStat stat)
    {
        float value = 1.5f;

        value +=
            stat.GetModifierValue(
                StatType.CriticalDamage,
                ModifierType.Flat);

        value *=
            1f +
            stat.GetModifierValue(
                StatType.CriticalDamage,
                ModifierType.Percent);

        return value;
    }

    #endregion

    #region Accuracy / Dodge

    public static float CalculateAccuracy(PlayerStat stat)
    {
        float value =
            90f +
            stat.TotalDex * 0.8f;

        value +=
            stat.GetModifierValue(
                StatType.Accuracy,
                ModifierType.Flat);

        value *=
            1f +
            stat.GetModifierValue(
                StatType.Accuracy,
                ModifierType.Percent);

        return value;
    }

    public static float CalculateDodge(PlayerStat stat)
    {
        float value =
            stat.TotalDex * 0.4f +
            stat.TotalLuck * 0.2f +
            stat.SumEquipmentFloat(
                item => item.bonusDodgeChance);

        value +=
            stat.GetModifierValue(
                StatType.Dodge,
                ModifierType.Flat);

        value *=
            1f +
            stat.GetModifierValue(
                StatType.Dodge,
                ModifierType.Percent);

        return value;
    }

    #endregion

    #region Drop

    public static float CalculateDropRate(PlayerStat stat)
    {
        float value =
            1f +
            stat.TotalLuck * 0.003f +
            stat.SumEquipmentFloat(
                item => item.bonusDropRate);

        value +=
            stat.GetModifierValue(
                StatType.DropRate,
                ModifierType.Flat);

        value *=
            1f +
            stat.GetModifierValue(
                StatType.DropRate,
                ModifierType.Percent);

        return value;
    }

    #endregion

    #region Projectile

    public static int CalculatePiercing(PlayerStat stat)
    {
        int value =
            stat.weaponSlot == null
                ? 0
                : stat.weaponSlot.basePiercingCount;

        value +=
            stat.SumEquipmentInt(
                item => item.bonusPiercing);

        value +=
            Mathf.RoundToInt(
                stat.GetModifierValue(
                    StatType.Piercing,
                    ModifierType.Flat));

        return value;
    }

    public static float CalculateDecay(PlayerStat stat)
    {
        if (stat.weaponSlot == null)
            return 0f;

        float value =
            stat.weaponSlot.baseDamageDecay;

        value -=
            stat.SumEquipmentFloat(
                item => item.bonusDecayReduction);

        value -=
            stat.GetModifierValue(
                StatType.DamageDecay,
                ModifierType.Flat);

        return Mathf.Max(0f, value);
    }

    #endregion
}