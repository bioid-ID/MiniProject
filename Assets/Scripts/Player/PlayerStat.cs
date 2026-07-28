using UnityEngine;
using System;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat Instance { get; private set; }

    [Header("[ Level & Exp ]")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentExp;

    public float MaxExp => (Mathf.Pow(currentLevel, 2.5f) * 80f) + 100f;

    [Header("[ Points ]")]
    [SerializeField] private int statPoints;
    [SerializeField] private int passivePoints;

    [Header("[ Base Stats ]")]
    [SerializeField] private int baseStr = 10;
    [SerializeField] private int baseDex = 10;
    [SerializeField] private int baseInt = 10;
    [SerializeField] private int baseLuck = 10;

    [Header("[ Current ]")]
    private float currentHp;
    private float currentMp;

    [Header("[ Equipment ]")]
    public EquipmentData weaponSlot;
    public EquipmentData subWeaponSlot;
    public EquipmentData helmetSlot;
    public EquipmentData armorSlot;
    public EquipmentData pantsSlot;
    public EquipmentData glovesSlot;
    public EquipmentData bootsSlot;
    public EquipmentData necklaceSlot;
    public EquipmentData ringSlot1;
    public EquipmentData ringSlot2;

    public int CurrentLevel => currentLevel;
    public float CurrentExp => currentExp;
    public int StatPoints => statPoints;
    public int PassivePoints => passivePoints;

    public float CurrentHp => currentHp;
    public float CurrentMp => currentMp;

    private EquipmentData[] EquippedItems =>
        new EquipmentData[]
        {
            weaponSlot, subWeaponSlot, helmetSlot, armorSlot, pantsSlot, 
            glovesSlot, bootsSlot, necklaceSlot, ringSlot1,ringSlot2
        };

    #region Primary Stats

    public int TotalStr => baseStr + SumEquipmentInt(item => item.bonusStr);

    public int TotalDex => baseDex + SumEquipmentInt(item => item.bonusDex);

    public int TotalInt => baseInt + SumEquipmentInt(item => item.bonusInt);

    public int TotalLuck => baseLuck + SumEquipmentInt(item => item.bonusLuck);

    #endregion

    #region Calculated Stats

    public float MaxHp => StatCalculator.CalculateMaxHp(this);

    public float MaxMp => StatCalculator.CalculateMaxMp(this);

    public float Defense => StatCalculator.CalculateDefense(this);

    public float AttackDamage => StatCalculator.CalculateAttackDamage(this);

    public float AttackRange => StatCalculator.CalculateAttackRange(this);

    public float AttackSpeed => StatCalculator.CalculateAttackSpeed(this);

    public float MoveSpeed => StatCalculator.CalculateMoveSpeed(this);

    public float CriticalChance => StatCalculator.CalculateCriticalChance(this);

    public float DodgeChance => StatCalculator.CalculateDodge(this);

    public float Accuracy => StatCalculator.CalculateAccuracy(this);

    public float DropRateMultiplier => StatCalculator.CalculateDropRate(this);

    public int TotalPiercingCount => StatCalculator.CalculatePiercing(this);

    public float FinalDamageDecay => StatCalculator.CalculateDecay(this);

    #endregion

    public AttackType CurrentAttackType
    {
        get
        {
            if (weaponSlot == null)
                return AttackType.Melee;

            return weaponSlot.AttackType;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        currentHp = MaxHp;
        currentMp = MaxMp;
    }

    #region Equipment Sum

    public int SumEquipmentInt(Func<EquipmentData, int> selector)
    {
        int total = 0;

        foreach (EquipmentData item in EquippedItems)
        {
            if (item == null)
                continue;

            total += selector(item);
        }

        return total;
    }

    public float SumEquipmentFloat(Func<EquipmentData, float> selector)
    {
        float total = 0;

        foreach (EquipmentData item in EquippedItems)
        {
            if (item == null)
                continue;

            total += selector(item);
        }

        return total;
    }

    #endregion

    #region Level

    public void GainExp(float amount)
    {
        currentExp += amount;

        while (currentExp >= MaxExp)
        {
            currentExp -= MaxExp;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;

        statPoints += 5;
        passivePoints++;

        currentHp = MaxHp;
        currentMp = MaxMp;

        Debug.Log($"Level Up! Lv.{currentLevel}");
    }

    public bool InvestStat(string statName)
    {
        if (statPoints <= 0)
            return false;

        switch (statName.ToUpper())
        {
            case "STR":
                baseStr++;
                break;

            case "DEX":
                baseDex++;
                break;

            case "INT":
                baseInt++;
                break;

            case "LUCK":
                baseLuck++;
                break;

            default:
                return false;
        }

        statPoints--;

        return true;
    }

    #endregion

    #region Equipment

    public void EquipItem(EquipmentData item)
    {
        if (item == null)
            return;

        if (currentLevel < item.requiredLevel)
        {
            Debug.LogWarning("레벨 부족");
            return;
        }

        switch (item.slotType)
        {
            case EquipmentSlot.Weapon:
                weaponSlot = item;
                break;

            case EquipmentSlot.SubWeapon:
                subWeaponSlot = item;
                break;

            case EquipmentSlot.Helmet:
                helmetSlot = item;
                break;

            case EquipmentSlot.Armor:
                armorSlot = item;
                break;

            case EquipmentSlot.Pants:
                pantsSlot = item;
                break;

            case EquipmentSlot.Gloves:
                glovesSlot = item;
                break;

            case EquipmentSlot.Boots:
                bootsSlot = item;
                break;

            case EquipmentSlot.Necklace:
                necklaceSlot = item;
                break;

            case EquipmentSlot.Ring1:
                ringSlot1 = item;
                break;

            case EquipmentSlot.Ring2:
                ringSlot2 = item;
                break;
        }

        currentHp = Mathf.Min(currentHp, MaxHp);
        currentMp = Mathf.Min(currentMp, MaxMp);
    }

    public void UnequipItem(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon:
                weaponSlot = null;
                break;

            case EquipmentSlot.SubWeapon:
                subWeaponSlot = null;
                break;

            case EquipmentSlot.Helmet:
                helmetSlot = null;
                break;

            case EquipmentSlot.Armor:
                armorSlot = null;
                break;

            case EquipmentSlot.Pants:
                pantsSlot = null;
                break;

            case EquipmentSlot.Gloves:
                glovesSlot = null;
                break;

            case EquipmentSlot.Boots:
                bootsSlot = null;
                break;

            case EquipmentSlot.Necklace:
                necklaceSlot = null;
                break;

            case EquipmentSlot.Ring1:
                ringSlot1 = null;
                break;

            case EquipmentSlot.Ring2:
                ringSlot2 = null;
                break;
        }

        currentHp = Mathf.Min(currentHp, MaxHp);
        currentMp = Mathf.Min(currentMp, MaxMp);
    }

    #endregion
}