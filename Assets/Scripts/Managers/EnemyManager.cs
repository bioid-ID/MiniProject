using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        enemies.Clear();
    }

    public void Register(Enemy enemy)
    {
        if (enemy == null || enemies.Contains(enemy))
            return;

        enemies.Add(enemy);
    }

    public void Unregister(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    public void ClearAll()
    {
        enemies.Clear();
    }

    public int CountAliveBySpawnEntry(int entryIndex)
    {
        if (entryIndex < 0)
            return 0;

        int count = 0;
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = enemies[i];
            if (enemy == null)
            {
                enemies.RemoveAt(i);
                continue;
            }

            if (enemy.gameObject.activeInHierarchy && enemy.SpawnEntryIndex == entryIndex)
                count++;
        }

        return count;
    }

    public int ActiveCount
    {
        get
        {
            int count = 0;
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = enemies[i];
                if (enemy == null)
                {
                    enemies.RemoveAt(i);
                    continue;
                }

                if (enemy.gameObject.activeInHierarchy)
                    count++;
            }

            return count;
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = enemies[i];
            if (enemy == null)
            {
                enemies.RemoveAt(i);
                continue;
            }

            if (!enemy.gameObject.activeInHierarchy)
                continue;

            enemy.Tick(dt);
        }
    }
}
