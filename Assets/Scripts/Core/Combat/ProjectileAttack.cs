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

        Projectile projectile = PoolManager.Instance.Get<Projectile>();

        if (projectile == null)
            return;

        PlayerVisual visual = GetComponent<PlayerVisual>();
        Vector2 fallback = visual != null ? visual.LastFacing : Vector2.right;
        Vector2 aimDirection = PlayerAim.GetAttackDirection(transform, fallback);

        if (spawnPoint != null)
        {
            spawnPoint.position = transform.position;
            PlayerAim.ApplyDirection(spawnPoint, aimDirection);
        }

        projectile.transform.SetPositionAndRotation(
            spawnPoint != null ? spawnPoint.position : transform.position,
            spawnPoint != null ? spawnPoint.rotation : transform.rotation);

        projectile.Launch(
            finalDamage,
            stat.TotalPiercingCount,
            stat.FinalDamageDecay,
            DamageType.Physical,
            TeamType.Player,
            aimDirection);
    }
}