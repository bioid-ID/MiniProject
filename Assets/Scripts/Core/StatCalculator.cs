using UnityEngine;

public static class StatCalculator
{
    public static float CalculateMaxHp(PlayerStat stat)
    {
        return 100f
            + (stat.CurrentLevel * 25f)
            + (stat.TotalStr * 15f)
            + stat.SumEquipmentFloat(item => item.bonusMaxHealth);
    }

    public static float CalculateMaxMp(PlayerStat stat)
    {
        return 50f
            + (stat.CurrentLevel * 10f)
            + (stat.TotalInt * 20f)
            + stat.SumEquipmentFloat(item => item.bonusMaxMana);
    }

    public static float CalculateDefense(PlayerStat stat)
    {
        return (stat.TotalStr * 0.5f)
            + stat.SumEquipmentFloat(item => item.bonusDefense);
    }

    public static float CalculateMoveSpeed(PlayerStat stat)
    {
        return 5f
            + (stat.TotalDex * 0.004f)
            + stat.SumEquipmentFloat(item => item.bonusMoveSpeed);
    }

    public static float CalculateAttackSpeed(PlayerStat stat)
    {
        float weaponSpeed = 1f;

        if (stat.weaponSlot != null)
            weaponSpeed = stat.weaponSlot.attackSpeed;

        return Mathf.Max(
            0.1f,
            weaponSpeed +
            stat.SumEquipmentFloat(item => item.bonusAttackSpeed));
    }

    public static float CalculateAttackRange(PlayerStat stat)
    {
        float range = 1.5f;

        if (stat.weaponSlot != null)
            range = stat.weaponSlot.attackRange;

        return range +
            stat.SumEquipmentFloat(item => item.bonusRange);
    }

    public static float CalculateAttackDamage(PlayerStat stat)
    {
        if (stat.weaponSlot == null)
            return 10f;

        return stat.weaponSlot.CalculateWeaponDamage(stat);
    }

    public static float CalculateCriticalChance(PlayerStat stat)
    {
        return 5.5f
            + stat.TotalLuck * 0.5f
            + stat.SumEquipmentFloat(item => item.bonusCritChance);
    }

    public static float CalculateDodge(PlayerStat stat)
    {
        return stat.TotalDex * 0.4f
            + stat.TotalLuck * 0.2f
            + stat.SumEquipmentFloat(item => item.bonusDodgeChance);
    }

    public static float CalculateAccuracy(PlayerStat stat)
    {
        return 90f + stat.TotalDex * 0.8f;
    }

    public static float CalculateDropRate(PlayerStat stat)
    {
        return 1f
            + stat.TotalLuck * 0.003f
            + stat.SumEquipmentFloat(item => item.bonusDropRate);
    }

    public static int CalculatePiercing(PlayerStat stat)
    {
        if (stat.weaponSlot == null)
            return 0;

        return stat.weaponSlot.basePiercingCount
            + stat.SumEquipmentInt(item => item.bonusPiercing);
    }

    public static float CalculateDecay(PlayerStat stat)
    {
        if (stat.weaponSlot == null)
            return 0f;

        float decay = stat.weaponSlot.baseDamageDecay;

        decay -= stat.SumEquipmentFloat(
            item => item.bonusDecayReduction);

        return Mathf.Max(0f, decay);
    }
}