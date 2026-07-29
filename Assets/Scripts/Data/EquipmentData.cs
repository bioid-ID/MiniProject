using UnityEngine;
using System.Collections.Generic;
public enum EquipmentSlot
{
    Weapon, SubWeapon, Helmet,
    Armor, Pants, Gloves,
    Boots,  Necklace, Ring1,  Ring2
}

public enum WeaponType
{
    Melee, Ranged, Magic, Throwable, Whip
}
public enum EquipmentGrade
{
    Normal, Magic, Rare, Epic,
    Unique, Legendary, Mythic
}

public enum AttackType
{
    Melee, Projectile
}

public enum EquipmentSet
{
    None,  RedKnight, Assassin, Wizard, 
    Hunter, Dragon, Demon, Angel
}

public abstract class EquipmentData : ScriptableObject
{

    [Header("Info")]

    public string equipmentName;

    public Sprite icon;

    [TextArea]
    public string description;

    public EquipmentSlot slotType;

    public EquipmentGrade grade;

    public int requiredLevel = 1;

    public int sellPrice = 100;

    [Header("Weapon")]

    public WeaponType weaponType;

    public float weaponAtk = 20;

    public float attackSpeed = 1;

    public float attackRange = 2;

    public float projectileSpeed = 12;

    public int basePiercingCount;

    [Range(0f, 1f)]
    public float baseDamageDecay = 0.2f;

    public float knockBack;

    public float criticalMultiplier = 1.5f;

    [Header("Base Stat")]

    public int bonusStr;

    public int bonusDex;

    public int bonusInt;

    public int bonusLuck;

    [Header("Combat")]

    public float bonusMaxHealth;

    public float bonusMaxMana;

    public float bonusDefense;

    public float bonusMoveSpeed;

    public float bonusAttackSpeed;

    public float bonusRange;

    public float bonusCritChance;

    public float bonusCritDamage;

    public float bonusDodgeChance;

    public float bonusAccuracy;

    public float bonusDropRate;

    public float bonusCooldownReduction;

    [Header("Special")]

    public int bonusProjectile;

    public int bonusPiercing;

    public float bonusDecayReduction;

    public float bonusLifeSteal;

    public float bonusReflect;

    public float bonusBurnChance;

    public float bonusFreezeChance;

    public float bonusPoisonChance;

    public float bonusShockChance;

    [Header("Scaling")]

    [Range(0, 3)]
    public float strCoefficient = 1;

    [Range(0, 3)]
    public float dexCoefficient;

    [Range(0, 3)]
    public float intCoefficient;

    [Range(0, 3)]
    public float luckCoefficient;


    public AttackType AttackType
    {
        get
        {
            switch (weaponType)
            {
                case WeaponType.Melee:
                case WeaponType.Whip:
                    return AttackType.Melee;

                case WeaponType.Ranged:
                case WeaponType.Magic:
                case WeaponType.Throwable:
                    return AttackType.Projectile;

                default:
                    return AttackType.Melee;
            }
        }
    }

    public float CalculateWeaponDamage(PlayerStat stat)
    {
        if (stat == null)
            return weaponAtk;

        return weaponAtk +
               stat.TotalStr * strCoefficient +
               stat.TotalDex * dexCoefficient +
               stat.TotalInt * intCoefficient +
               stat.TotalLuck * luckCoefficient;
    }
    [Header("Buff")]

    public List<BuffBase> buffs = new();
}