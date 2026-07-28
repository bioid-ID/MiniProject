using UnityEngine;

public class ProjectileAttack : AttackBase
{
    [SerializeField] private Transform spawnPoint;

    private PlayerStat stat;

    protected override void Awake()
    {
        base.Awake();
        stat = PlayerStat.Instance;
    }

    public override void Attack(float finalDamage)
    {
        if (!CanAttack())
            return;

        if (spawnPoint == null)
            return;

        ResetCooldown();

        Projectile projectile = PoolManager.Instance.GetProjectile();

        projectile.transform.SetPositionAndRotation(
            spawnPoint.position,
            spawnPoint.rotation);

        projectile.Launch(
        finalDamage,
        stat.TotalPiercingCount,
        stat.FinalDamageDecay,
        DamageType.Physical,
        TeamType.Player);
    }
}