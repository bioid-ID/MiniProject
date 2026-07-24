using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat Instance { get; private set; }

    [Header("[ Level & Exp System ]")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentExp = 0f;
    public float MaxExp => (Mathf.Pow(currentLevel, 2.5f) * 80f) + 100f;

    [Header("[ Remaining Points ]")]
    [SerializeField] private int statPoints = 0;
    [SerializeField] private int passivePoints = 0;

    [Header("[ Core Stats (Base) ]")]
    [SerializeField] private int baseStr = 10;
    [SerializeField] private int baseDex = 10;
    [SerializeField] private int baseInt = 10;
    [SerializeField] private int baseLuck = 10;

    [Header("[ Current Status ]")]
    private float currentHp;
    private float currentMp;

    [Header("[ 10 Equipped Slots ]")]
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

    private IEnumerable<EquipmentData> EquippedItems
    {
        get
        {
            if (weaponSlot != null) yield return weaponSlot;
            if (subWeaponSlot != null) yield return subWeaponSlot;
            if (helmetSlot != null) yield return helmetSlot;
            if (armorSlot != null) yield return armorSlot;
            if (pantsSlot != null) yield return pantsSlot;
            if (glovesSlot != null) yield return glovesSlot;
            if (bootsSlot != null) yield return bootsSlot;
            if (necklaceSlot != null) yield return necklaceSlot;
            if (ringSlot1 != null) yield return ringSlot1;
            if (ringSlot2 != null) yield return ringSlot2;
        }
    }

    public int TotalStr => baseStr + SumInt(item => item.bonusStr);
    public int TotalDex => baseDex + SumInt(item => item.bonusDex);
    public int TotalInt => baseInt + SumInt(item => item.bonusInt);
    public int TotalLuck => baseLuck + SumInt(item => item.bonusLuck);

    public float MaxHp => 100f + (currentLevel * 25f) + (TotalStr * 15f) + SumFloat(item => item.bonusMaxHealth);
    public float MaxMp => 50f + (currentLevel * 10f) + (TotalInt * 20f) + SumFloat(item => item.bonusMaxMana);
    public float Defense => (TotalStr * 0.5f) + SumFloat(item => item.bonusDefense);

    // 이제 타입이 PlayerStat으로 일치하므로 정상 작동합니다.
    public float AttackDamage => weaponSlot == null ? 10f : weaponSlot.CalculateFinalWeaponDamage(this);
    public float AttackRange => (weaponSlot == null ? 1.5f : weaponSlot.attackRange) + SumFloat(item => item.bonusRange);
    public float AttackSpeed => Mathf.Max(0.1f, (weaponSlot == null ? 1.0f : weaponSlot.attackSpeed) + SumFloat(item => item.bonusAttackSpeed));
    public float MoveSpeed => 5.0f + (TotalDex / 25f * 0.1f) + SumFloat(item => item.bonusMoveSpeed);

    public float CriticalChance => 5.5f + (TotalLuck * 0.5f) + SumFloat(item => item.bonusCritChance);
    public float DodgeChance => 0f + (TotalDex * 0.4f) + (TotalLuck * 0.2f) + SumFloat(item => item.bonusDodgeChance);
    public float Accuracy => 90f + (TotalDex * 0.8f);
    public float DropRateMultiplier => 1.0f + (TotalLuck * 0.003f) + SumFloat(item => item.bonusDropRate);

    public int TotalPiercingCount => (weaponSlot == null ? 0 : weaponSlot.basePiercingCount) + SumInt(item => item.bonusPiercing);
    public float FinalDamageDecay
    {
        get
        {
            if (weaponSlot == null) return 0f;
            float baseDecay = weaponSlot.baseDamageDecay;
            float reduction = SumFloat(item => item.bonusDecayReduction);
            return Mathf.Max(0f, baseDecay - reduction);
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentHp = MaxHp;
        currentMp = MaxMp;
    }

    private int SumInt(Func<EquipmentData, int> selector)
    {
        int total = 0;
        foreach (var item in EquippedItems) total += selector(item);
        return total;
    }

    private float SumFloat(Func<EquipmentData, float> selector)
    {
        float total = 0;
        foreach (var item in EquippedItems) total += selector(item);
        return total;
    }

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
        passivePoints += 1;
        currentHp = MaxHp;
        currentMp = MaxMp;
        Debug.Log($"Level Up! 현재 레벨: {currentLevel}, 스탯포인트: {statPoints}");
    }

    public bool InvestStat(string statName)
    {
        if (statPoints <= 0) return false;
        switch (statName.ToUpper())
        {
            case "STR": baseStr++; break;
            case "DEX": baseDex++; break;
            case "INT": baseInt++; break;
            case "LUCK": baseLuck++; break;
            default: return false;
        }
        statPoints--;
        return true;
    }

    public void EquipItem(EquipmentData item)
    {
        if (item == null) return;
        if (currentLevel < item.requiredLevel)
        {
            Debug.LogWarning($"{item.equipmentName} 장착 실패: 레벨이 부족합니다. (요구: {item.requiredLevel})");
            return;
        }

        switch (item.slotType)
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
        currentHp = Mathf.Min(currentHp, MaxHp);
        currentMp = Mathf.Min(currentMp, MaxMp);
    }
    public void UnequipItem(EquipmentSlot slotType)
    {
        switch (slotType)
        {
            case EquipmentSlot.Weapon: weaponSlot = null; break;
            case EquipmentSlot.SubWeapon: subWeaponSlot = null; break;
            case EquipmentSlot.Helmet: helmetSlot = null; break;
            case EquipmentSlot.Armor: armorSlot = null; break;
            case EquipmentSlot.Pants: pantsSlot = null; break;
            case EquipmentSlot.Gloves: glovesSlot = null; break;
            case EquipmentSlot.Boots: bootsSlot = null; break;
            case EquipmentSlot.Necklace: necklaceSlot = null; break;
            case EquipmentSlot.Ring1: ringSlot1 = null; break;
            case EquipmentSlot.Ring2: ringSlot2 = null; break;
        }
        currentHp = Mathf.Min(currentHp, MaxHp);
        currentMp = Mathf.Min(currentMp, MaxMp);
    }

}
