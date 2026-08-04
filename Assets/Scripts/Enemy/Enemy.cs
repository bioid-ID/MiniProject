using UnityEngine;

public class Enemy : Character
{
    [Header("Enemy")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private int level = 1;

    private EnemyMovement movement;
    private EnemyAttack attack;
    private EnemyHealth health;
    private Transform player;
    private EnemyState currentState;
    private float stunTimer;
    private int spawnEntryIndex = -1;
    private string visualKey;

    public EnemyData Data => enemyData;
    public int SpawnEntryIndex => spawnEntryIndex;
    public EnemyState CurrentState => currentState;
    public AttackType AttackType =>
        enemyData != null ? enemyData.attackType : AttackType.Melee;

    private float DetectRange => enemyData != null ? enemyData.detectRange : 8f;
    private float AttackRange => enemyData != null ? enemyData.attackRange : 1.5f;
    private float StopDistanceFactor => enemyData != null ? enemyData.stopDistanceFactor : 0.85f;

    protected override void Awake()
    {
        base.Awake();

        movement = GetComponent<EnemyMovement>();
        attack = GetComponent<EnemyAttack>();
        health = GetComponent<EnemyHealth>();

        if (health == null)
            health = GetComponentInChildren<EnemyHealth>();

        ResolvePlayerTarget();
    }

    public override void OnSpawn()
    {
        base.OnSpawn();

        stunTimer = 0f;
        ResolvePlayerTarget();

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.Register(this);

        Initialize(level);
        ChangeState(EnemyState.Idle);
    }

    public override void OnDespawn()
    {
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.Unregister(this);

        stunTimer = 0f;
        spawnEntryIndex = -1;
        visualKey = null;
        transform.localScale = Vector3.one;
        base.OnDespawn();
    }

    public void ApplyData(EnemyData data, string resourcesVisualKey = null, int entryIndex = -1)
    {
        enemyData = data;
        visualKey = resourcesVisualKey;
        spawnEntryIndex = entryIndex;
        EnemyVisualUtility.ApplyBody(this, enemyData, visualKey);
    }

    public void BindSpawnEntry(int entryIndex, string resourcesVisualKey)
    {
        spawnEntryIndex = entryIndex;
        visualKey = resourcesVisualKey;
    }

    public void Initialize(int stageLevel)
    {
        level = stageLevel;
        EnemyVisualUtility.ApplyBody(this, enemyData, visualKey);

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

        if (enemyData.isBoss)
        {
            transform.localScale = Vector3.one * enemyData.bossScale;
            attack?.ConfigureAsBoss(enemyData);
        }
    }

    public void ApplyHitReaction(Vector2 direction, float knockbackForce, float stunDuration)
    {
        if (currentState == EnemyState.Dead)
            return;

        float kbMult = enemyData != null ? enemyData.knockbackTakenMult : 1f;
        float stunMult = enemyData != null ? enemyData.stunTakenMult : 1f;

        knockbackForce *= kbMult;
        stunDuration *= stunMult;

        if (knockbackForce > 0f)
            movement?.ApplyKnockback(direction, knockbackForce);

        if (stunDuration <= 0f)
            return;

        stunTimer = Mathf.Max(stunTimer, stunDuration);
        movement?.ApplyStun(stunDuration);
        ChangeState(EnemyState.Stunned);
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

        ResolvePlayerTarget();

        if (stunTimer > 0f)
        {
            stunTimer -= deltaTime;
            if (stunTimer > 0f)
            {
                currentState = EnemyState.Stunned;
                movement?.Stop();
                return;
            }

            stunTimer = 0f;
            if (currentState == EnemyState.Stunned)
                ChangeState(EnemyState.Chase);
        }

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
            case EnemyState.Stunned:
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
            case EnemyState.Attack:
            case EnemyState.Stunned:
            case EnemyState.Dead:
                movement?.Stop();
                break;
            case EnemyState.Chase:
                break;
        }
    }

    private void UpdateIdle()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= DetectRange)
            ChangeState(EnemyState.Chase);
    }

    private void UpdateChase()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > DetectRange * 1.15f)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        float stopDistance = AttackRange * StopDistanceFactor;
        if (distance <= stopDistance)
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        movement.Move(dir);
    }

    private void UpdateAttack()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > AttackRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        movement.Stop();
        attack.Attack();
    }

    private void ResolvePlayerTarget()
    {
        if (player != null)
            return;

        if (PlayerManager.Instance != null)
        {
            player = PlayerManager.Instance.transform;
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
    }
}
