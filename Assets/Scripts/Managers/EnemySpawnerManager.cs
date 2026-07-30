using System.Collections;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    public static EnemySpawnerManager Instance { get; private set; }

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private bool spawnImmediatelyOnDeath = true;
    [SerializeField] private int currentStageLevel = 1;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(PeriodicSpawnRoutine());
    }

    private IEnumerator PeriodicSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            SpawnAtRandom();
        }
    }

    public void OnMonsterKilled(Vector3 deadPos)
    {
        DungeonManager.Instance.LogKill();

        PlayerData.Instance.AddExp(20f);

        if (!spawnImmediatelyOnDeath)
            return;

        Spawn(deadPos);
    }

    private void SpawnAtRandom()
    {
        if (spawnPoints.Length == 0)
            return;

        int index = Random.Range(0, spawnPoints.Length);

        Spawn(spawnPoints[index].position);
    }

    private void Spawn(Vector3 position)
    {
        Enemy enemy =
    PoolManager.Instance.Get<Enemy>(
        PoolKey.Enemy);

        enemy.transform.SetPositionAndRotation(
            position,
            Quaternion.identity);

        enemy.Initialize(currentStageLevel);
    }
}