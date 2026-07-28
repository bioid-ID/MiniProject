using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private readonly List<Enemy> enemies = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Register(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void Unregister(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        for (int i = 0; i < enemies.Count; i++)
        {
            Enemy enemy = enemies[i];

            if (!enemy.gameObject.activeInHierarchy)
                continue;

            enemy.Tick(dt);
        }
    }
}