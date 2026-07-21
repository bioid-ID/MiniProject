using UnityEditor.SearchService;
using UnityEngine;

public class ProjectileAttack : AttackBase
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPoint;

    public override void ExecuteAttack()
    {
        if(projectilePrefab == null || spawnPoint == null) return;

        GameObject projGo = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

        Projectile projectile = projGo.GetComponent<Projectile>();
        if(projectile != null)
        {
            projectile.Launch(damage);
        }
    }
}