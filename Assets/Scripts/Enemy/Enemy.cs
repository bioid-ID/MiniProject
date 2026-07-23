using UnityEngine;
using UnityEngine.TextCore.Text;

public class Enemy : Character
{
    [Header("Enemy Setting")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private int currentLevel = 1;

    private float maxHp;
    private float currentHp;
    private float attack;
    private float defense;

    private void Start()
    {
        InitEnemy(currentLevel);
    }

    public void InitEnemy(int stageLevel)
    {
        currentLevel = stageLevel;

        if (enemyData == null)
        {
            Debug.LogError($"{gameObject.name}에 EnemyData가 할당되지 않았습니다.");
            return;
        }

        float bossMultiplier = enemyData.isBoss ? 2.0f : 1.0f;
        float levelFactor = currentLevel - 1;

        maxHp = enemyData.baseHp * Mathf.Pow(1f + enemyData.hpGrowthRate * bossMultiplier, levelFactor);
        attack = enemyData.baseAttack * Mathf.Pow(1f + enemyData.attackGrowthRate * bossMultiplier, levelFactor);
        defense = enemyData.baseDefense * Mathf.Pow(1f + enemyData.defenseGrowthRate * bossMultiplier, levelFactor);

        currentHp = maxHp;

        Debug.Log($"[{enemyData.enemyName} Lv.{currentLevel}] 스폰 - HP: {maxHp:F0}, ATK: {attack:F0}, DEF: {defense:F0}");
    }
    public float GetAttackDamage()
    {
        return attack;
    }
    public void TakeDamage(float incomingDamage)
    {
        float damageReduction = 100f / (100f + defense);
        float finalDamage = incomingDamage * damageReduction;

        finalDamage = Mathf.Max(1f, finalDamage);

        currentHp -= finalDamage;
        Debug.Log($"{enemyData.enemyName}이(가) {finalDamage:F1}의 피해를 입음. (남은 체력: {currentHp:F0}/{maxHp:F0})");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{enemyData.enemyName} 사망! ");
        Destroy(gameObject);
    }
}
