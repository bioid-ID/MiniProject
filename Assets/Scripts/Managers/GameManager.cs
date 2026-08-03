using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Enemy enemyPrefab;

    [SerializeField]
    private Projectile projectilePrefab;

    [SerializeField]
    private Loot lootPrefab;

    private void Start()
    {
        if (PoolManager.Instance == null)
        {
            Debug.LogError("GameManager: PoolManager.Instance is missing.");
            return;
        }

        PoolManager.Instance.RegisterPool(enemyPrefab, 80);
        PoolManager.Instance.RegisterPool(projectilePrefab, 150);

        if (lootPrefab != null)
            PoolManager.Instance.RegisterPool(lootPrefab, 30);
    }
}