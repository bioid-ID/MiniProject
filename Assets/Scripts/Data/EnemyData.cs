using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public bool isBoss;

    [Header("Base Stats (Stage 1 / Level 1)")]
    public float baseHp = 100f;
    public float baseAttack = 10f;
    public float baseDefense = 5f;

    [Header("Growth Rates per Level (0.1 = 10%)")]
    public float hpGrowthRate = 0.2f;      
    public float attackGrowthRate = 0.12f;  
    public float defenseGrowthRate = 0.15f;
}
