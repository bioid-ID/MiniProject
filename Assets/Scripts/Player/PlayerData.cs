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

/// <summary>
/// �� ��ȯ �Ŀ��� �����Ǵ� ���൵ �����.
/// ���� �÷��� �� ��ġ�� PlayerStat�� �����Դϴ�.
/// </summary>
public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    [Header("Progress (persisted)")]
    public int Gold;
    public int Level = 1;
    public float CurrentExp;
    public int StatPoints;
    public int PassivePoints;

    [Header("Legacy (lobby UI��, ���� ����)")]
    public int InvestedAttack;
    public int InvestedHp;
    public int InvestedPassiveAttack;
    public Item Weapon = new Item { itemName = "�⺻ ��", baseStat = 15, goldCost = 150 };
    public Skill FireBall = new Skill { skillName = "���̾", baseDamage = 40 };

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveFrom(PlayerStat stat)
    {
        if (stat == null)
            return;

        Gold = stat.Gold;
        Level = stat.CurrentLevel;
        CurrentExp = stat.CurrentExp;
        StatPoints = stat.StatPoints;
        PassivePoints = stat.PassivePoints;
    }

    public void ApplyTo(PlayerStat stat)
    {
        if (stat == null)
            return;

        stat.LoadProgress(Gold, Level, CurrentExp, StatPoints, PassivePoints);
    }

    public void AddExp(float amount)
    {
        if (PlayerStat.Instance != null)
            PlayerStat.Instance.GainExp(amount);
    }

    public int GetTotalAttack()
    {
        int baseAtk = 10 + InvestedAttack * 2 + Weapon.GetCurrentStat();
        float multiplier = 1f + (InvestedPassiveAttack * 0.05f);
        return Mathf.RoundToInt(baseAtk * multiplier);
    }
}
