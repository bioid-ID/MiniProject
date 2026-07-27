using UnityEngine;

[CreateAssetMenu(menuName = "Data/Weapon")]
public class WeaponData : EquipmentData
{
    [Header("Weapon")]

    public WeaponType weaponType;

    public float attackDamage = 20;

    public float attackRange = 2;

    public float attackSpeed = 1;

    public int piercingCount;

    [Range(0, 1)]
    public float damageDecay;

    [Header("Scaling")]

    [Range(0, 3)]
    public float strScaling = 1;

    [Range(0, 3)]
    public float dexScaling;

    [Range(0, 3)]
    public float intScaling;

    [Range(0, 3)]
    public float luckScaling;

    [Header("Prefab")]

    public Projectile projectilePrefab;

    public Hitbox hitboxPrefab;
}