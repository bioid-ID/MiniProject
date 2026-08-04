using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Hitbox meleeHitbox;

    private bool bossMode;
    private int bossComboHits = 3;
    private float bossComboGap = 0.2f;
    private float attackCooldown = 1f;
    private float meleeHitDuration = 0.15f;
    private float attackKnockback = 1.5f;
    private float attackStun = 0.1f;

    private Enemy enemy;
    private float cooldownTimer;
    private bool comboRunning;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        if (meleeHitbox != null)
            meleeHitbox.gameObject.SetActive(false);

        if (attackPoint == null)
            attackPoint = transform;
    }

    private void OnEnable()
    {
        ApplyFromData(enemy != null ? enemy.Data : null);
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public void ApplyFromData(EnemyData data)
    {
        if (data == null)
            return;

        attackCooldown = data.attackCooldown;
        meleeHitDuration = data.meleeHitDuration;
        attackKnockback = data.attackKnockback;
        attackStun = data.attackStun;

        if (data.isBoss)
            ConfigureAsBoss(data);
    }

    public void ConfigureAsBoss(EnemyData data = null)
    {
        bossMode = true;
        EnemyData src = data != null ? data : enemy?.Data;
        if (src == null)
            return;

        attackCooldown = src.bossAttackCooldown;
        bossComboHits = src.bossComboHits;
        bossComboGap = src.bossComboGap;
        attackKnockback = src.attackKnockback;
        attackStun = src.attackStun;
        meleeHitDuration = src.meleeHitDuration;
    }

    public void Attack()
    {
        if (cooldownTimer > 0f || comboRunning)
            return;

        // Refresh in case data was applied after spawn.
        if (enemy != null && enemy.Data != null && !bossMode)
            ApplyFromData(enemy.Data);

        cooldownTimer = attackCooldown;

        if (bossMode)
        {
            StartCoroutine(BossComboRoutine());
            return;
        }

        switch (enemy.AttackType)
        {
            case AttackType.Melee:
                PerformMeleeAttack();
                break;
            case AttackType.Projectile:
                PerformProjectileAttack();
                break;
        }
    }

    private System.Collections.IEnumerator BossComboRoutine()
    {
        comboRunning = true;
        int hits = Mathf.Max(1, bossComboHits);
        float gap = Mathf.Max(0.05f, bossComboGap);

        for (int i = 0; i < hits; i++)
        {
            if (i % 2 == 0)
                PerformMeleeAttack(1f + i * 0.15f);
            else
                PerformProjectileAttack(1.1f + i * 0.1f);

            if (i < hits - 1)
                yield return new WaitForSeconds(gap);
        }

        comboRunning = false;
    }

    private void PerformMeleeAttack(float damageMultiplier = 1f)
    {
        if (meleeHitbox == null)
            return;

        Sprite swing = enemy != null && enemy.Data != null ? enemy.Data.meleeAttackSprite : null;
        EnemyVisualUtility.EnsureMeleeSprite(meleeHitbox, swing);

        Vector2 dir = ResolveAimDirection();
        DamageInfo damageInfo = new DamageInfo(
            gameObject,
            enemy.AttackPower * damageMultiplier,
            DamageType.Physical,
            TeamType.Enemy,
            knockbackForce: attackKnockback,
            stunDuration: attackStun,
            hitDirection: dir,
            attackMethod: AttackMethod.Melee);

        meleeHitbox.Initialize(damageInfo);
        meleeHitbox.gameObject.SetActive(true);

        CancelInvoke(nameof(DisableHitbox));
        Invoke(nameof(DisableHitbox), meleeHitDuration);
    }

    private void DisableHitbox()
    {
        if (meleeHitbox != null)
            meleeHitbox.gameObject.SetActive(false);
    }

    private void PerformProjectileAttack(float damageMultiplier = 1f)
    {
        if (attackPoint == null || PoolManager.Instance == null)
            return;

        AimAtPlayer();

        Projectile projectile = PoolManager.Instance.Get<Projectile>();
        if (projectile == null)
            return;

        projectile.transform.SetPositionAndRotation(
            attackPoint.position,
            attackPoint.rotation);

        projectile.Launch(
            enemy.AttackPower * damageMultiplier,
            enemy.Data != null ? enemy.Data.projectilePiercing : 0,
            enemy.Data != null ? enemy.Data.projectileDecay : 0f,
            DamageType.Physical,
            TeamType.Enemy,
            attackPoint.right,
            AttackMethod.Projectile,
            attackKnockback,
            attackStun);

        if (enemy.Data != null)
            EnemyVisualUtility.ApplyProjectileSprite(projectile, enemy.Data.projectileSprite);
    }

    private void AimAtPlayer()
    {
        Vector2 dir = ResolveAimDirection();
        if (attackPoint != null)
            PlayerAim.ApplyDirection(attackPoint, dir);
    }

    private Vector2 ResolveAimDirection()
    {
        Transform player = PlayerManager.Instance != null
            ? PlayerManager.Instance.transform
            : GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            return Vector2.right;

        Vector2 origin = attackPoint != null ? (Vector2)attackPoint.position : (Vector2)transform.position;
        Vector2 dir = ((Vector2)player.position - origin).normalized;
        return dir.sqrMagnitude < 0.001f ? Vector2.right : dir;
    }

    private void OnDisable()
    {
        cooldownTimer = 0f;
        comboRunning = false;
        bossMode = false;
        StopAllCoroutines();
        CancelInvoke();

        if (meleeHitbox != null)
            meleeHitbox.gameObject.SetActive(false);
    }
}
