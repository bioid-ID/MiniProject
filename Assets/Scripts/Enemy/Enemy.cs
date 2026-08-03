using UnityEngine;

public class Enemy : Character
{
    [Header("Enemy")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private int level = 1;

    [SerializeField] private float fallbackDetectRange = 8f;
    [SerializeField] private float fallbackAttackRange = 1.5f;

    private EnemyMovement movement;
    private EnemyAttack attack;
    private EnemyHealth health;
    private Transform player;

    private EnemyState currentState;

    public EnemyState CurrentState => currentState;
    public AttackType AttackType =>
        enemyData != null ? enemyData.attackType : AttackType.Melee;

    private float DetectRange =>
        enemyData != null ? enemyData.detectRange : fallbackDetectRange;

    private float AttackRange =>
        enemyData != null ? enemyData.attackRange : fallbackAttackRange;

    protected override void Awake()
    {
        base.Awake();

        movement = GetComponent<EnemyMovement>();
        attack = GetComponent<EnemyAttack>();
        health = GetComponent<EnemyHealth>();

        if (health == null)
            health = GetComponentInChildren<EnemyHealth>();

        if (PlayerManager.Instance != null)
            player = PlayerManager.Instance.transform;
    }

    public override void OnSpawn()
    {
        base.OnSpawn();

        if (player == null && PlayerManager.Instance != null)
            player = PlayerManager.Instance.transform;

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.Register(this);

        Initialize(level);

        ChangeState(EnemyState.Idle);
    }

    public override void OnDespawn()
    {
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.Unregister(this);

        base.OnDespawn();
    }

    public void Initialize(int stageLevel)
    {
        level = stageLevel;

        if (enemyData == null)
        {
            SetStats(40f + stageLevel * 15f, 8f + stageLevel * 2f, 2f + stageLevel, 0f);
            movement.Initialize(3f);
            health.Initialize(MaxHp);
            return;
        }

        float boss = enemyData.isBoss ? 2f : 1f;
        float lv = level - 1;

        float hp =
            enemyData.baseHp *
            Mathf.Pow(1f + enemyData.hpGrowthRate * boss, lv);

        float atk =
            enemyData.baseAttack *
            Mathf.Pow(1f + enemyData.attackGrowthRate * boss, lv);

        float def =
            enemyData.baseDefense *
            Mathf.Pow(1f + enemyData.defenseGrowthRate * boss, lv);

        SetStats(hp, atk, def, 0f);

        movement.Initialize(enemyData.moveSpeed);

        health.Initialize(MaxHp);
    }

    protected override void Die()
    {
        ChangeState(EnemyState.Dead);

        if (DropManager.Instance != null)
            DropManager.Instance.Drop(enemyData, transform.position);

        if (PoolManager.Instance != null)
            PoolManager.Instance.Return(this);
    }

    public void Tick(float deltaTime)
    {
        if (!gameObject.activeSelf)
            return;

        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;

            case EnemyState.Chase:
                UpdateChase();
                break;

            case EnemyState.Attack:
                UpdateAttack();
                break;

            case EnemyState.Dead:
                break;
        }
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        switch (currentState)
        {
            case EnemyState.Idle:
                movement.Stop();
                break;

            case EnemyState.Chase:
                break;

            case EnemyState.Attack:
                movement.Stop();
                break;

            case EnemyState.Dead:
                movement.Stop();
                break;
        }
    }

    private void UpdateIdle()
    {
        if (player == null)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                player.position);

        if (distance <= DetectRange)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    private void UpdateChase()
    {
        if (player == null)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                player.position);

        if (distance > DetectRange)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        if (distance <= AttackRange)
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        Vector2 dir =
            (player.position - transform.position).normalized;

        movement.Move(dir);
    }

    private void UpdateAttack()
    {
        if (player == null)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                player.position);

        if (distance > AttackRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        movement.Stop();

        attack.Attack();
    }
}