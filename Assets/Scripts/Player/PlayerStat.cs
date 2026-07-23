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

    [Header("Current Equipped Weapon")]
    // 현재 장착 중인 무기 데이터 (무기가 바뀔 때마다 이 슬롯에 할당)
    public WeaponData equippedWeapon;

    // --- 종합 스탯 계산 (캐릭터 순수 베이스 스탯 + 장착 장비의 보너스 스탯) ---
    public int TotalStr => baseStr;
    public int TotalDex => baseDex;
    public int TotalInt => baseInt;
    public int TotalLuck => baseLuck + (equippedWeapon != null ? equippedWeapon.bonusLuck : 0); // 장비의 운 보정 반영

    // --- 기획서 반영: 실시간 최종 전투 능력치 연산 속성 (Properties) ---
    public int Level => level;

    // 최종 최대 체력 = 기본 체력 성장 + STR 보너스 + [장비 고유 체력 증가량]
    public float MaxHealth
    {
        get
        {
            float weaponHpMod = (equippedWeapon != null) ? equippedWeapon.bonusMaxHealth : 0f;
            return 100f + (level * 25f) + (TotalStr * 15f) + weaponHpMod;
        }
    }

    // 최종 최대 마나 = 기본 마나 성장 + INT 보너스 + [장비 고유 마나 증가량]
    public float MaxMana
    {
        get
        {
            float weaponMpMod = (equippedWeapon != null) ? equippedWeapon.bonusMaxMana : 0f;
            return 50f + (level * 10f) + (TotalInt * 20f) + weaponMpMod;
        }
    }

    // 최종 사거리 = 무기 기본 사거리 + [장비 추가 사거리 보정값]
    public float AttackRange
    {
        get
        {
            if (equippedWeapon == null) return 1.5f; // 맨손 사거리
            return equippedWeapon.attackRange + equippedWeapon.bonusRange;
        }
    }

    // 최종 공격 속도 = 무기 기본 공속 + [장비 추가 공속 보정값]
    public float AttackSpeed
    {
        get
        {
            float weaponAsMod = (equippedWeapon != null) ? equippedWeapon.bonusAttackSpeed : 0f;
            float baseAs = (equippedWeapon != null) ? equippedWeapon.attackSpeed : 1.0f;
            return Mathf.Max(0.1f, baseAs + weaponAsMod); // 공속이 0 이하로 떨어지는 것 방지
        }
    }

    // 최종 이동 속도 = 캐릭터 기본(5.0) + DEX 보너스 + [장비 고유 이속 보정값]
    public float MoveSpeed
    {
        get
        {
            float weaponSpeedMod = (equippedWeapon != null) ? equippedWeapon.bonusMoveSpeed : 0f;
            return 5.0f + (TotalDex / 25f * 0.1f) + weaponSpeedMod;
        }
    }

    // 최종 치명타 확률 = 기본(5%) + LUCK 보너스 + [장비 고유 치명타 보정값]
    public float CriticalChance
    {
        get
        {
            float weaponCritMod = (equippedWeapon != null) ? equippedWeapon.bonusCritChance : 0f;
            return 5.0f + (TotalLuck * 0.5f) + weaponCritMod;
        }
    }

    // 현재 장착된 무기 기준 '실시간 최종 공격 데미지' 계산 반환
    public float AttackDamage
    {
        get
        {
            if (equippedWeapon == null) return 10f; // 맨손 데미지 기본값
            return equippedWeapon.CalculateFinalWeaponDamage(this);
        }
    }

    // 추가 스탯 목록 (회피, 명중, 드랍율)
    public float DodgeChance => (TotalDex * 0.4f) + (TotalLuck * 0.2f);
    public float Accuracy => 90f + (TotalDex * 0.8f);
    public float DropRateMultiplier => 1.0f + (TotalLuck * 0.003f);

    public int StatPoints => statPoints;
    public int PassivePoints => passivePoints;

    public void AddExp(int amount)
    {
        currentExp += amount;
        while (currentExp >= maxExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentExp -= maxExp;
        level++;
        maxExp = Mathf.RoundToInt(maxExp * 1.2f);

        statPoints += 5;
        passivePoints += 1;

        if (PlayerManager.Instance != null && PlayerManager.Instance.Health != null)
        {
            PlayerManager.Instance.Health.Heal(MaxHealth);
        }

        Debug.Log($"Level Up! 현재 레벨: {level}, 스탯 포인트: {statPoints}");
    }

    // 스탯 투자 시스템
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
