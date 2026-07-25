using System.Collections;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    public GameObject monsterPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    public bool spawnImmediatelyOnDeath = true;

    void Start()
    {
        StartCoroutine(PeriodicSpawnRoutine());
    }

    IEnumerator PeriodicSpawnRoutine()
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

        if (spawnImmediatelyOnDeath)
        {
            Instantiate(monsterPrefab, deadPos, Quaternion.identity);
        }
    }

    private void SpawnAtRandom()
    {
        if (spawnPoints.Length == 0) return;
        int idx = Random.Range(0, spawnPoints.Length);
        Instantiate(monsterPrefab, spawnPoints[idx].position, Quaternion.identity);
    }
}
