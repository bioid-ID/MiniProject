using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Enemy enemyPrefab;

    [SerializeField]
    private Projectile projectilePrefab;

    private void Start()
    {
        PoolManager.Instance.RegisterPool(
            PoolKey.Enemy,
            enemyPrefab,
            80);

        PoolManager.Instance.RegisterPool(
            PoolKey.Projectile,
            projectilePrefab,
            150);
    }
}