using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [Header("Projectile Pool")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int projectilePoolSize = 100;

    [Header("Enemy Pool")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private int enemyPoolSize = 50;

    private ObjectPool<Projectile> projectilePool;
    private ObjectPool<Enemy> enemyPool;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("PoolManager가 이미 존재합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializePools();
    }

    private void InitializePools()
    {
        projectilePool = new ObjectPool<Projectile>(
            projectilePrefab,
            projectilePoolSize,
            transform);

        projectilePool.Initialize();

        enemyPool = new ObjectPool<Enemy>(
            enemyPrefab,
            enemyPoolSize,
            transform);

        enemyPool.Initialize();
    }

    #region Projectile

    public Projectile GetProjectile()
    {
        return projectilePool.Get();
    }

    public void ReturnProjectile(Projectile projectile)
    {
        projectilePool.Return(projectile);
    }

    #endregion

    #region Enemy

    public Enemy GetEnemy()
    {
        return enemyPool.Get();
    }

    public void ReturnEnemy(Enemy enemy)
    {
        enemyPool.Return(enemy);
    }

    #endregion
}