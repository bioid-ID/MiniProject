using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터 1종류 = 에셋 1개. 드랍/스탯/이미지는 전부 여기에.
/// </summary>
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string enemyName;
    public bool isBoss;

    [Header("Visuals (드래그해서 넣기)")]
    [Tooltip("적 몸 스프라이트. 비우면 Resources/Enemies 또는 기본 원")]
    public Sprite bodySprite;
    [Tooltip("근접 공격 순간에 보이는 스윙/슬래시 이미지")]
    public Sprite meleeAttackSprite;
    [Tooltip("원거리 탄환 이미지")]
    public Sprite projectileSprite;
    public Color bodyTint = Color.white;

    [Header("Base Stats (Level 1)")]
    public float baseHp = 100f;
    public float baseAttack = 10f;
    public float baseDefense = 5f;

    [Header("Growth per Level (0.1 = +10%)")]
    public float hpGrowthRate = 0.2f;
    public float attackGrowthRate = 0.12f;
    public float defenseGrowthRate = 0.15f;

    [Header("Movement / AI")]
    public float moveSpeed = 3f;
    public float detectRange = 8f;
    public float attackRange = 1.5f;
    [Tooltip("Chase stops at attackRange * this")]
    public float stopDistanceFactor = 0.85f;

    [Header("Attack")]
    public AttackType attackType = AttackType.Melee;
    public float attackCooldown = 1f;
    public float meleeHitDuration = 0.15f;
    public float attackKnockback = 1.5f;
    public float attackStun = 0.1f;
    public int projectilePiercing;
    public float projectileDecay;

    [Header("When this enemy is hit (resist)")]
    [Tooltip("1 = normal, 0.5 = half knockback, 0 = immune")]
    public float knockbackTakenMult = 1f;
    public float stunTakenMult = 1f;

    [Header("Rewards / Drop (몬스터마다 다름)")]
    public int goldReward = 50;
    public float expReward = 20f;
    public List<DropEntry> dropTable = new();

    [Header("Boss only")]
    public float bossScale = 1.6f;
    public int bossComboHits = 3;
    public float bossComboGap = 0.2f;
    public float bossAttackCooldown = 1.35f;
}
