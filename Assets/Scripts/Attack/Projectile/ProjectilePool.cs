using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int initialSize = 50;

    private readonly Queue<Projectile> pool = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        for (int i = 0; i < initialSize; i++)
        {
            AddProjectile();
        }
    }

    private Projectile AddProjectile()
    {
        Projectile projectile = Instantiate(projectilePrefab, transform);

        projectile.gameObject.SetActive(false);

        pool.Enqueue(projectile);

        return projectile;
    }

    public Projectile Get()
    {
        if (pool.Count == 0)
        {
            AddProjectile();
        }

        Projectile projectile = pool.Dequeue();

        projectile.gameObject.SetActive(true);

        return projectile;
    }

    public void Release(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);

        projectile.transform.SetParent(transform);

        pool.Enqueue(projectile);
    }
}