using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private int level = 1;

    private EnemyHealth health;

    private void Awake()
    {
        base.Awake();
        health = GetComponent<EnemyHealth>();
    }

    private void Start()
    {
        Initialize(level);
    }

    public void Initialize(int stageLevel)
    {
        level = stageLevel;

        float bossMultiplier = enemyData.isBoss ? 2f : 1f;
        float lv = level - 1;

        float hp =
            enemyData.baseHp *
            Mathf.Pow(1 + enemyData.hpGrowthRate * bossMultiplier, lv);

        float atk =
            enemyData.baseAttack *
            Mathf.Pow(1 + enemyData.attackGrowthRate * bossMultiplier, lv);

        float def =
            enemyData.baseDefense *
            Mathf.Pow(1 + enemyData.defenseGrowthRate * bossMultiplier, lv);

        SetStats(hp, atk, def);

        health.Initialize(MaxHp);
    }
}