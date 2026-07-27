using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [Header("Pools")]

    [SerializeField]
    private Projectile projectilePrefab;

    [SerializeField]
    private int projectilePoolSize = 100;

    private ObjectPool<Projectile> projectilePool;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        projectilePool = new ObjectPool<Projectile>(
            projectilePrefab,
            projectilePoolSize,
            transform);

        projectilePool.Initialize();
    }

    public Projectile GetProjectile()
    {
        return projectilePool.Get();
    }

    public void ReturnProjectile(Projectile projectile)
    {
        projectilePool.Return(projectile);
    }
}