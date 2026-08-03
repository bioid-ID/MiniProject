using UnityEngine;
using System;
using System.Collections.Generic;
public class PlayerStat : MonoBehaviour
{
    public static PlayerStat Instance { get; private set; }

    [Header("[ Level & Exp ]")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentExp;

    public float MaxExp => (Mathf.Pow(currentLevel, 2.5f) * 80f) + 100f;

    [Header("[ Currency ]")]
    [SerializeField] private int gold;

    public int Gold => gold;

    [Header("[ Points ]")]
    [SerializeField] private int statPoints;
    [SerializeField] private int passivePoints;

    [Header("[ Base Stats ]")]
    [SerializeField] private int baseStr = 10;
    [SerializeField] private int baseDex = 10;
    [SerializeField] private int baseInt = 10;
    [SerializeField] private int baseLuck = 10;
    private readonly List<StatModifier> modifiers = new();
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
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        if (PlayerData.Instance != null)
            PlayerData.Instance.ApplyTo(this);
        else
        {
            currentHp = MaxHp;
            currentMp = MaxMp;
        }

        SaveManager.Instance?.ApplyEquipmentToCurrentPlayer();
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        gold += amount;
    }

    public void LoadProgress(int loadGold, int level, float exp, int statPts, int passivePts)
    {
        gold = loadGold;
        currentLevel = Mathf.Max(1, level);
        currentExp = Mathf.Max(0f, exp);
        statPoints = statPts;
        passivePoints = passivePts;
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
        PlayerData.Instance?.SaveFrom(this);
        SaveManager.Instance?.Save();
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

    public void ClearAllEquipment()
    {
        weaponSlot = null;
        subWeaponSlot = null;
        helmetSlot = null;
        armorSlot = null;
        pantsSlot = null;
        glovesSlot = null;
        bootsSlot = null;
        necklaceSlot = null;
        ringSlot1 = null;
        ringSlot2 = null;
        currentHp = Mathf.Min(currentHp, MaxHp);
        currentMp = Mathf.Min(currentMp, MaxMp);
    }

    public void EquipItem(EquipmentData item)
    {
        if (item == null)
            return;

        if (currentLevel < item.requiredLevel)
        {
            Debug.LogWarning("???? ????");
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
        ApplyEquipmentBuff(item);
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
        RemoveEquipmentBuff(slot);
        currentHp = Mathf.Min(currentHp, MaxHp);
        currentMp = Mathf.Min(currentMp, MaxMp);
    }

    #endregion

    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
    }
    public void RemoveModifier(object source)
    {
        modifiers.RemoveAll(x => x.Source == source);
    }

    public void RemoveSource(object source)
    {
        modifiers.RemoveAll(x => x.Source == source);
    }

    public float GetModifierValue(
      StatType stat,
      ModifierType type)
    {
        float total = 0f;

        foreach (StatModifier modifier in modifiers)
        {
            if (modifier.Stat != stat)
                continue;

            if (modifier.Type != type)
                continue;

            total += modifier.Value;
        }

        return total;
    }
    private void ApplyEquipmentBuff(EquipmentData item)
    {
        if (item == null)
            return;

        foreach (BuffBase buff in item.buffs)
        {
            if (buff == null)
                continue;

            BuffManager.Instance.AddBuff(buff);
        }
    }
    private void RemoveEquipmentBuff(EquipmentSlot slot)
    {
        EquipmentData item = GetEquipment(slot);

        if (item == null)
            return;

        foreach (BuffBase buff in item.buffs)
        {
            if (buff == null)
                continue;

            BuffManager.Instance.RemoveBuff(buff);
        }
    }
    private EquipmentData GetEquipment(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon: return weaponSlot;
            case EquipmentSlot.SubWeapon: return subWeaponSlot;
            case EquipmentSlot.Helmet: return helmetSlot;
            case EquipmentSlot.Armor: return armorSlot;
            case EquipmentSlot.Pants: return pantsSlot;
            case EquipmentSlot.Gloves: return glovesSlot;
            case EquipmentSlot.Boots: return bootsSlot;
            case EquipmentSlot.Necklace: return necklaceSlot;
            case EquipmentSlot.Ring1: return ringSlot1;
            case EquipmentSlot.Ring2: return ringSlot2;
        }

        return null;
    }
}