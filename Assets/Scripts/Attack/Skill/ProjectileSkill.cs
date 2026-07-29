using UnityEngine;

public class ProjectileSkill : SkillBase
{
    [SerializeField]
    private Transform firePoint;

    protected override void Use()
    {
        Projectile projectile =
            PoolManager.Instance.GetProjectile();

        projectile.transform.SetPositionAndRotation(
            firePoint.position,
            firePoint.rotation);

        projectile.Launch(
            data.damage,
            0,
            0f,
            DamageType.Physical,
            TeamType.Player);
    }
}