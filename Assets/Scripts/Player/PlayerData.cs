using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public int level = 1;
    public int baseStat = 10;
    public int goldCost = 100;

    public int GetCurrentStat() => baseStat + (level - 1) * 5;
    public int GetUpgradeCost() => goldCost * level;
    public void Upgrade() => level++;
}

[System.Serializable]
public class Skill
{
    public string skillName;
    public bool isUnlocked = false;
    public int level = 1;
    public int baseDamage = 50;

    public int GetCurrentDamage() => baseDamage + (level - 1) * 15;
    public void Unlock() => isUnlocked = true;
    public void Upgrade() => level++;
}

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    [Header("Currency & Exp")]
    public int Gold = 0;
    public int Level = 1;
    public float CurrentExp = 0f;
    public float MaxExp = 100f;

    [Header("Stats Point")]
    public int StatPoints = 0;
    public int InvestedAttack = 0;
    public int InvestedHp = 0;

    [Header("Passive Tree")]
    public int InvestedPassiveAttack = 0; 

    [Header("Equipment & Skills")]
    public Item Weapon = new Item { itemName = "기본 검", baseStat = 15, goldCost = 150 };
    public Skill FireBall = new Skill { skillName = "파이어볼", baseDamage = 40 };

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void AddExp(float amount)
    {
        CurrentExp += amount;
        if (CurrentExp >= MaxExp)
        {
            CurrentExp -= MaxExp;
            Level++;
            StatPoints += 5; 
            MaxExp *= 1.2f; 
            Debug.Log($"레벨업! 현재 레벨: {Level}");
        }
    }

    public int GetTotalAttack()
    {
        int baseAtk = 10 + InvestedAttack * 2 + Weapon.GetCurrentStat();
        float multiplier = 1f + (InvestedPassiveAttack * 0.05f);
        return Mathf.RoundToInt(baseAtk * multiplier);
    }
}
