using UnityEngine;

public class ProjectileSkill : SkillBase
{
    [SerializeField]
    private Transform firePoint;

    private void Start()
    {
        if (firePoint == null)
        {
            Transform found = transform.parent != null
                ? transform.parent.Find("FirePoint")
                : null;

            if (found != null)
                firePoint = found;
        }
    }

    protected override void Use()
    {
        if (data == null || PoolManager.Instance == null)
            return;

        Projectile projectile =
            PoolManager.Instance.Get<Projectile>();

        if (projectile == null || firePoint == null)
            return;

        Transform owner = transform.parent != null ? transform.parent : transform;
        PlayerVisual visual = owner.GetComponent<PlayerVisual>();
        Vector2 fallback = visual != null ? visual.LastFacing : Vector2.right;
        Vector2 aimDirection = PlayerAim.GetAttackDirection(owner, fallback, data.range);

        firePoint.position = owner.position;
        PlayerAim.ApplyDirection(firePoint, aimDirection);

        projectile.transform.SetPositionAndRotation(
            firePoint.position,
            firePoint.rotation);

        float damage = data.damage;
        if (PlayerStat.Instance != null)
            damage = Mathf.Max(data.damage, PlayerStat.Instance.AttackDamage);

        projectile.Launch(
            damage,
            0,
            0f,
            DamageType.Physical,
            TeamType.Player,
            aimDirection);
    }
}