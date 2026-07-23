using UnityEngine;

// 기획하신 무기별 공격 형태 분류
public enum WeaponType
{
    Melee,          // 근접 무기 (검, 창 등 - 범위 공격)
    Ranged,         // 원거리 무기 (활, 석궁, 총, 발리스타 등 - 투사체 공격)
    Magic,          // 마법 무기 (지팡이, 오브 등 - 원거리 범위 공격)
    Throwable,      // 투척 무기 (다트, 표창, 손도끼, 투척창 등)
    Whip            // 채찍형 무기 (랜덤 범위 공격)
}

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Identity")]
    public string weaponName;           // 무기 이름
    public WeaponType weaponType;       // 무기 종류

    [Header("Base Weapon Stats")]
    public float weaponAtk = 20f;       // 무기 자체 기본 공격력
    public float attackRange = 2f;      // 무기 기본 사거리
    public float attackSpeed = 1f;      // 무기 기본 공격 속도

    [Header("Weapon Stat Modifiers (장착 시 플레이어 스탯 증감)")]
    [Tooltip("체력 보정 (예: 생명력의 검 +50)")] public float bonusMaxHealth = 0f;
    [Tooltip("마나 보정 (예: 마법 지팡이 +30)")] public float bonusMaxMana = 0f;
    [Tooltip("이동속도 보정 (예: 무거운 대검 -0.8, 단검 +0.4)")] public float bonusMoveSpeed = 0f;
    [Tooltip("공격속도 보정 (예: 가벼운 무기 +0.2)")] public float bonusAttackSpeed = 0f;
    [Tooltip("치명타 확률 보정 (예: 암살용 무기 +15)")] public float bonusCritChance = 0f;
    [Tooltip("추가 사거리 보정 (예: 긴 창 +1.5)")] public float bonusRange = 0f;
    [Tooltip("순수 운 스탯 보정 (예: 행운의 주사위 +5)")] public int bonusLuck = 0;

    [Header("Damage Scaling Coefficients (스탯별 데미지 반영 계수)")]
    [Range(0f, 3f)] public float strCoefficient = 1.0f; // STR(힘) 영향도
    [Range(0f, 3f)] public float dexCoefficient = 0.0f; // DEX(민첩) 영향도
    [Range(0f, 3f)] public float intCoefficient = 0.0f; // INT(지능) 영향도
    [Range(0f, 3f)] public float luckCoefficient = 0.0f; // 질문하신 LUCK(운) 데미지 영향도!

    /// <summary>
    /// 현재 플레이어의 종합 스탯을 기반으로 이 무기의 최종 공격력을 계산합니다.
    /// </summary>
    public float CalculateFinalWeaponDamage(PlayerStat playerStat)
    {
        // 각 스탯과 무기 고유 계수를 곱해 보너스 데미지 산출
        float scalingDmg = (playerStat.TotalStr * strCoefficient) +
                            (playerStat.TotalDex * dexCoefficient) +
                            (playerStat.TotalInt * intCoefficient) +
                            (playerStat.TotalLuck * luckCoefficient); 

        return weaponAtk + scalingDmg;
    }
}
