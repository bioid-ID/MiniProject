using UnityEngine;

public enum EquipmentSlot
{
    Weapon, SubWeapon, Helmet, Armor, Pants, Gloves, Boots, Necklace, Ring1, Ring2
}

[CreateAssetMenu(fileName = "NewEquipment", menuName = "ScriptableObjects/ExpandedEquipment")]
public class EquipmentData : ScriptableObject
{
    [Header("[ Equipment Identity ]")]
    public string equipmentName;
    public EquipmentSlot slotType;
    public int requiredLevel = 1;

    [Header("[ 무기 고유 능력치 (무기 슬롯인 경우만 사용) ]")]
    public WeaponType weaponType;
    public float weaponAtk = 20f;
    public float attackRange = 2f;
    public float attackSpeed = 1f;

    [Tooltip("무기 고유 관통 횟수 (0이면 관통 불가, 1이면 1명 관통 후 2명째에서 소멸)")]
    public int basePiercingCount = 0;

    [Tooltip("무기 고유 관통 시 데미지 감쇠율 (0.2면 관통할 때마다 데미지 20%씩 감소)")]
    [Range(0f, 1f)] public float baseDamageDecay = 0.2f;

    [Header("[ 1. 핵심 스탯 보정 ]")]
    public int bonusStr = 0; public int bonusDex = 0; public int bonusInt = 0; public int bonusLuck = 0;

    [Header("[ 2. 전투 능력치 보정 ]")]
    public float bonusMaxHealth = 0f; public float bonusMaxMana = 0f; public float bonusDefense = 0f;
    public float bonusMoveSpeed = 0f; public float bonusAttackSpeed = 0f; public float bonusRange = 0f;

    [Header("[ 3. 관통 및 확률 스탯 보정 (New!) ]")]
    [Tooltip("장비로 추가되는 관통 횟수 (예: 관통의 화살통 +1)")]
    public int bonusPiercing = 0;

    [Tooltip("관통 데미지 감쇠 완화 수치 (예: 0.05면 관통 대미지 깎이는 폭을 5% 줄여줌)")]
    public float bonusDecayReduction = 0f;

    public float bonusCritChance = 0f;
    public float bonusDodgeChance = 0f;
    public float bonusDropRate = 0f;

    [Header("[ 4. 데미지 반영 계수 ]")]
    [Range(0f, 3f)] public float strCoefficient = 1.0f;
    [Range(0f, 3f)] public float dexCoefficient = 0.0f;
    [Range(0f, 3f)] public float intCoefficient = 0.0f;
    [Range(0f, 3f)] public float luckCoefficient = 0.0f;

    public float CalculateFinalWeaponDamage(PlayerStat playerStat)
    {
        float scalingDmg = (playerStat.TotalStr * strCoefficient) +
                            (playerStat.TotalDex * dexCoefficient) +
                            (playerStat.TotalInt * intCoefficient) +
                            (playerStat.TotalLuck * luckCoefficient);
        return weaponAtk + scalingDmg;
    }
}