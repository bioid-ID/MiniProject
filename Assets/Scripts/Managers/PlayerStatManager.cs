using UnityEngine;
using System;

public class PlayerStatManager : MonoBehaviour
{
    public static PlayerStatManager Instance { get; private set; }

    [Header("Level & Exp")]
    public int currentLevel = 1;
    public float currentExp = 0f;
    public float maxExp => (Mathf.Pow(currentLevel, 2.5f) * 80f) + 100f; // 레벨업 요구량 공식

    [Header("Remaining Points")]
    public int statPoints = 0;
    public int passivePoints = 0;

    [Header("Core Stats (Base)")]
    public int baseStr = 10;
    public int baseDex = 10;
    public int baseInt = 10;
    public int baseLuck = 10;

    // 장비나 버프로 추가되는 보너스 스탯 변수 (외부에서 가감 가능)
    [Header("Bonus Stats (Equipment/Buffs)")]
    public int bonusStr;
    public int bonusDex;
    public int bonusInt;
    public int bonusLuck;
    public float equipmentAtk; // 장비 무기 자체 공격력
    public float equipmentDef; // 장비 방어구 자체 방어력

    // 최종 합산 스탯 속성 (Property)
    public int TotalStr => baseStr + bonusStr;
    public int TotalDex => baseDex + bonusDex;
    public int TotalInt => baseInt + bonusInt;
    public int TotalLuck => baseLuck + bonusLuck;

    // --- 기획서 반영 최종 전투 스탯 계산 ---
    public float MaxHp => 100f + (currentLevel * 25f) + (TotalStr * 15f); // 레벨당 25, STR당 15
    public float MaxMp => 50f + (currentLevel * 10f) + (TotalInt * 20f);  // 레벨당 10, INT당 20

    public float Defense => equipmentDef + (TotalStr * 0.5f); // STR로 방어력 소폭 보정
    public float MoveSpeed => 5.0f + (TotalDex / 25f * 0.1f);  // DEX 25당 이속 0.1 증가

    // 확률형 스탯 (퍼센트 단위)
    public float CriticalChance => 5.5f + (TotalLuck * 0.5f);   // 기본 5% + LUCK당 0.5%
    public float DodgeChance => 0f + (TotalDex * 0.4f) + (TotalLuck * 0.2f); // DEX당 0.4% + LUCK당 0.2%
    public float Accuracy => 90f + (TotalDex * 0.8f);           // 기본 90% + DEX당 0.8%
    public float DropRateMultiplier => 1.0f + (TotalLuck * 0.003f); // LUCK당 0.3% 증가 (단리)

    private float currentHp;
    private float currentMp;

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

    // 경험치 획득 및 레벨업 체크
    public void GainExp(float amount)
    {
        currentExp += amount;
        while (currentExp >= maxExp)
        {
            currentExp -= maxExp;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        statPoints += 5;       // 스탯 포인트 +5
        passivePoints += 1;    // 패시브 포인트 +1

        // 레벨업 시 체력/마나 풀 회복
        currentHp = MaxHp;
        currentMp = MaxMp;

        Debug.Log($"Level Up! 현재 레벨: {currentLevel}, 스탯포인트: {statPoints}");
    }

    // 스탯 투자 메서드 (UI 버튼과 연동)
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
}
