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

    public float HpRegen => StatCalculator.CalculateHpRegen(this);
    public float MpRegen => StatCalculator.CalculateMpRegen(this);
    public float LifeSteal => StatCalculator.CalculateLifeSteal(this);
    public float ManaSteal => StatCalculator.CalculateManaSteal(this);

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

    private void Update()
    {
        TickRegen(Time.deltaTime);
    }

    private void TickRegen(float deltaTime)
    {
        if (deltaTime <= 0f)
            return;

        if (GameStateController.Instance != null && !GameStateController.Instance.IsPlaying)
            return;

        float hpRegen = HpRegen * deltaTime;
        if (hpRegen > 0f)
        {
            PlayerHealth health = GetComponent<PlayerHealth>();
            if (health != null)
                health.Heal(hpRegen, HealSource.Regen);
            else
                currentHp = Mathf.Min(currentHp + hpRegen, MaxHp);
        }

        float mpRegen = MpRegen * deltaTime;
        if (mpRegen > 0f)
            RestoreMp(mpRegen, ManaSource.Regen);
    }

    public void RestoreMp(float amount, ManaSource source = ManaSource.Other)
    {
        if (amount <= 0f)
            return;

        float before = currentMp;
        currentMp = Mathf.Min(currentMp + amount, MaxMp);
        float actual = currentMp - before;
        if (actual > 0f)
            DungeonManager.Instance?.RunStats.LogMana(actual, source);
    }

    public bool SpendMp(float amount)
    {
        if (amount <= 0f)
            return true;

        if (currentMp < amount)
            return false;

        currentMp -= amount;
        return true;
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

    /// <summary>
    /// Roguelike entry: keep gold only, reset combat progress to starter values.
    /// </summary>
    public void ApplyRoguelikeFreshStart(bool keepGold)
    {
        var bal = GameBalance.Config; // BALANCE
        int keptGold = keepGold ? gold : 0;
        ClearAllEquipment();
        baseStr = bal.roguelikeBaseStr;
        baseDex = bal.roguelikeBaseDex;
        baseInt = bal.roguelikeBaseInt;
        baseLuck = bal.roguelikeBaseLuck;
        LoadProgress(keptGold, level: 1, exp: 0f, statPts: 0, passivePts: 0);
    }

    public void SetGold(int amount)
    {
        gold = Mathf.Max(0, amount);
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
            Debug.LogWarning("Required level not met.");
            return;
        }

        EquipmentSlot slot = item.slotType;
        EquipmentData previous = GetEquipped(slot);
        if (previous != null)
            RemoveEquipmentBuffFromItem(previous);

        SetEquipped(slot, item);
        ApplyEquipmentBuff(item);
        currentHp = Mathf.Min(currentHp, MaxHp);
        currentMp = Mathf.Min(currentMp, MaxMp);
    }

    public bool TryEquipFromInventory(EquipmentData item)
    {
        if (item == null || Inventory.Instance == null)
            return false;

        if (!Inventory.Instance.HasItem(item, 1))
            return false;

        if (currentLevel < item.requiredLevel)
            return false;

        EquipmentSlot slot = ResolveEquipSlot(item);
        EquipmentData previous = GetEquipped(slot);

        Inventory.Instance.RemoveItem(item, 1);

        if (previous != null)
        {
            RemoveEquipmentBuffFromItem(previous);
            Inventory.Instance.AddItem(previous, 1);
        }

        SetEquipped(slot, item);
        ApplyEquipmentBuff(item);
        currentHp = Mathf.Min(currentHp, MaxHp);
        currentMp = Mathf.Min(currentMp, MaxMp);
        PlayerData.Instance?.SaveFrom(this);
        SaveManager.Instance?.Save();
        Inventory.Instance.NotifyChanged();
        return true;
    }

    public bool TryUnequipToInventory(EquipmentSlot slot)
    {
        EquipmentData equipped = GetEquipped(slot);
        if (equipped == null || Inventory.Instance == null)
            return false;

        if (!Inventory.Instance.AddItem(equipped, 1))
            return false;

        RemoveEquipmentBuffFromItem(equipped);
        SetEquipped(slot, null);
        currentHp = Mathf.Min(currentHp, MaxHp);
        currentMp = Mathf.Min(currentMp, MaxMp);
        PlayerData.Instance?.SaveFrom(this);
        SaveManager.Instance?.Save();
        Inventory.Instance.NotifyChanged();
        return true;
    }

    public EquipmentData GetEquipped(EquipmentSlot slot)
    {
        return GetEquipment(slot);
    }

    public void UnequipItem(EquipmentSlot slot)
    {
        EquipmentData item = GetEquipment(slot);
        if (item != null)
            RemoveEquipmentBuffFromItem(item);

        SetEquipped(slot, null);
        currentHp = Mathf.Min(currentHp, MaxHp);
        currentMp = Mathf.Min(currentMp, MaxMp);
    }

    private static EquipmentSlot ResolveEquipSlot(EquipmentData item)
    {
        if (item.slotType == EquipmentSlot.Ring1 || item.slotType == EquipmentSlot.Ring2)
        {
            if (PlayerStat.Instance != null && PlayerStat.Instance.ringSlot1 == null)
                return EquipmentSlot.Ring1;
            if (PlayerStat.Instance != null && PlayerStat.Instance.ringSlot2 == null)
                return EquipmentSlot.Ring2;
            return EquipmentSlot.Ring1;
        }

        return item.slotType;
    }

    private void SetEquipped(EquipmentSlot slot, EquipmentData item)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon: weaponSlot = item; break;
            case EquipmentSlot.SubWeapon: subWeaponSlot = item; break;
            case EquipmentSlot.Helmet: helmetSlot = item; break;
            case EquipmentSlot.Armor: armorSlot = item; break;
            case EquipmentSlot.Pants: pantsSlot = item; break;
            case EquipmentSlot.Gloves: glovesSlot = item; break;
            case EquipmentSlot.Boots: bootsSlot = item; break;
            case EquipmentSlot.Necklace: necklaceSlot = item; break;
            case EquipmentSlot.Ring1: ringSlot1 = item; break;
            case EquipmentSlot.Ring2: ringSlot2 = item; break;
        }
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
        if (item == null || item.buffs == null || BuffManager.Instance == null)
            return;

        foreach (BuffBase buff in item.buffs)
        {
            if (buff == null)
                continue;

            BuffManager.Instance.AddBuff(buff);
        }
    }
    private void RemoveEquipmentBuffFromItem(EquipmentData item)
    {
        if (item == null || item.buffs == null || BuffManager.Instance == null)
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