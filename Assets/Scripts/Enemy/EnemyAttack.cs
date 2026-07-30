using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Hitbox meleeHitbox;

    [Header("Attack")]
    [SerializeField] private float attackDuration = 0.15f;
    [SerializeField] private float attackCooldown = 1f;

    private Enemy enemy;

    private float cooldownTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        if (meleeHitbox != null)
            meleeHitbox.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public void Attack()
    {
        if (cooldownTimer > 0f)
            return;

        cooldownTimer = attackCooldown;

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

    private void PerformMeleeAttack()
    {
        if (meleeHitbox == null)
            return;

        DamageInfo damageInfo = new DamageInfo(
            gameObject,
            enemy.AttackPower,
            DamageType.Physical,
            TeamType.Enemy);

        meleeHitbox.Initialize(damageInfo);

        meleeHitbox.gameObject.SetActive(true);

        CancelInvoke(nameof(DisableHitbox));
        Invoke(nameof(DisableHitbox), attackDuration);
    }

    private void DisableHitbox()
    {
        if (meleeHitbox != null)
            meleeHitbox.gameObject.SetActive(false);
    }

    private void PerformProjectileAttack()
    {
        if (attackPoint == null)
            return;

        Projectile projectile = PoolManager.Instance.Get<Projectile>(PoolKey.Projectile);

        projectile.transform.SetPositionAndRotation(
            attackPoint.position,
            attackPoint.rotation);

        projectile.Launch(
            enemy.AttackPower,
            0,
            0f,
            DamageType.Physical,
            TeamType.Enemy);
    }

    private void OnDisable()
    {
        cooldownTimer = 0f;

        CancelInvoke();

        if (meleeHitbox != null)
            meleeHitbox.gameObject.SetActive(false);
    }
}