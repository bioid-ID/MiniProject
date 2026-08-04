using System.Collections.Generic;
using UnityEngine;

public class DungeonRunTracker
{
    public float TotalDamageDealt { get; private set; }
    public float TotalDamageTaken { get; private set; }
    public float TotalHealing { get; private set; }
    public float TotalManaRestored { get; private set; }
    public int KilledMonsters { get; private set; }
    public int GoldEarned { get; private set; }
    public int CritHits { get; private set; }
    public int HitsLanded { get; private set; }

    public readonly Dictionary<AttackMethod, float> DamageByMethod = new();
    public readonly Dictionary<DamageType, float> DamageByType = new();
    public readonly Dictionary<HealSource, float> HealingBySource = new();
    public readonly Dictionary<ManaSource, float> ManaBySource = new();

    public void Reset()
    {
        TotalDamageDealt = 0f;
        TotalDamageTaken = 0f;
        TotalHealing = 0f;
        TotalManaRestored = 0f;
        KilledMonsters = 0;
        GoldEarned = 0;
        CritHits = 0;
        HitsLanded = 0;
        DamageByMethod.Clear();
        DamageByType.Clear();
        HealingBySource.Clear();
        ManaBySource.Clear();
    }

    public void LogDamageDealt(float amount, AttackMethod method, DamageType type, bool isCritical)
    {
        if (amount <= 0f)
            return;

        TotalDamageDealt += amount;
        HitsLanded++;
        if (isCritical)
            CritHits++;

        Add(DamageByMethod, method, amount);
        Add(DamageByType, type, amount);
    }

    public void LogDamageTaken(float amount)
    {
        if (amount <= 0f)
            return;

        TotalDamageTaken += amount;
    }

    public void LogHeal(float amount, HealSource source)
    {
        if (amount <= 0f)
            return;

        TotalHealing += amount;
        Add(HealingBySource, source, amount);
    }

    public void LogMana(float amount, ManaSource source)
    {
        if (amount <= 0f)
            return;

        TotalManaRestored += amount;
        Add(ManaBySource, source, amount);
    }

    public void LogKill() => KilledMonsters++;

    public void LogGold(int amount)
    {
        if (amount <= 0)
            return;

        GoldEarned += amount;
    }

    public DungeonRunResult BuildResult(string title, int bonusGold = 0)
    {
        if (bonusGold > 0)
            LogGold(bonusGold);

        return new DungeonRunResult
        {
            Title = title,
            Kills = KilledMonsters,
            GoldEarned = GoldEarned,
            DamageDealt = TotalDamageDealt,
            DamageTaken = TotalDamageTaken,
            Healing = TotalHealing,
            ManaRestored = TotalManaRestored,
            CritHits = CritHits,
            HitsLanded = HitsLanded,
            DamageByMethod = Clone(DamageByMethod),
            DamageByType = Clone(DamageByType),
            HealingBySource = Clone(HealingBySource),
            ManaBySource = Clone(ManaBySource)
        };
    }

    private static void Add<T>(Dictionary<T, float> map, T key, float amount)
    {
        if (map.ContainsKey(key))
            map[key] += amount;
        else
            map[key] = amount;
    }

    private static Dictionary<T, float> Clone<T>(Dictionary<T, float> source)
    {
        return new Dictionary<T, float>(source);
    }
}
