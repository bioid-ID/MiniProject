using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [Header("Level System")]
    [SerializeField] private int level = 1;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int maxExp = 100;

    [Header("Remaining Points")]
    [SerializeField] private int statPoints = 0;
    [SerializeField] private int passivePoints = 0;

    [Header("Core Stats (Base)")]
    [SerializeField] private int baseStr = 10;
    [SerializeField] private int baseDex = 10;
    [SerializeField] private int baseInt = 10;
    [SerializeField] private int baseLuck = 10;

    [Header("Bonus Stats (Equipment)")]
    public int bonusStr;
    public int bonusDex;
    public int bonusInt;
    public int bonusLuck;
    public float equipmentAtk; // 장착 무기 공격력
    public float equipmentDef; // 장착 방어구 방어력

    // 합산 스탯 프로퍼티
    public int TotalStr => baseStr + bonusStr;
    public int TotalDex => baseDex + bonusDex;
    public int TotalInt => baseInt + bonusInt;
    public int TotalLuck => baseLuck + bonusLuck;

    // --- 기획서 반영 실시간 최종 스탯 계산 ---
    public int Level => level;
    public float MaxHealth => 100f + (level * 25f) + (TotalStr * 15f); // STR 반영 체력
    public float MaxMana => 50f + (level * 10f) + (TotalInt * 20f);   // INT 반영 마나
    public float Defense => equipmentDef + (TotalStr * 0.5f);         // 방어력
    public float MoveSpeed => 5.0f + (TotalDex / 25f * 0.1f);         // DEX 반영 이속

    // 확률 스탯 (%)
    public float CriticalChance => 5.0f + (TotalLuck * 0.5f);
    public float DodgeChance => (TotalDex * 0.4f) + (TotalLuck * 0.2f);
    public float Accuracy => 90f + (TotalDex * 0.8f);
    public float DropRateMultiplier => 1.0f + (TotalLuck * 0.003f);

    public int StatPoints => statPoints;
    public int PassivePoints => passivePoints;

    public void AddExp(int amount)
    {
        currentExp += amount;
        // 요구 경험치량이 꽉 차면 반복 레벨업 가능하도록 while문 권장
        while (currentExp >= maxExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentExp -= maxExp;
        level++;

        // 기존 경험치 상승 공식 유지
        maxExp = Mathf.RoundToInt(maxExp * 1.2f);

        // 레벨업 보상 포인트 지급 (기획서 내용 반영)
        statPoints += 5;
        passivePoints += 1;

        // 기존에 작성해두신 PlayerManager를 통한 풀피 회복 로직 유지
        if (PlayerManager.Instance != null && PlayerManager.Instance.Health != null)
        {
            PlayerManager.Instance.Health.Heal(MaxHealth);
        }

        Debug.Log($"Level Up! 현재 레벨: {level}, 스탯 포인트: {statPoints}");
    }

    // UI에서 스탯 올릴 때 사용할 함수
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
