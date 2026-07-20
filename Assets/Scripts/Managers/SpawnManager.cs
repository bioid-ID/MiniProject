using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject monsterPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f; 
    public bool spawnImmediately = false;

    void Start()
    {
        if (!spawnImmediately)
            StartCoroutine(SpawnTimerRoutine());
    }

    IEnumerator SpawnTimerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnMonster();
        }
    }

    public void OnMonsterDead(Vector3 deadPosition)
    {
        if (spawnImmediately)
        {
            SpawnMonsterAtPosition(deadPosition);
        }
    }

    private void SpawnMonster()
    {
        int randIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(monsterPrefab, spawnPoints[randIndex].position, Quaternion.identity);
    }

    private void SpawnMonsterAtPosition(Vector3 position)
    {
        Instantiate(monsterPrefab, position, Quaternion.identity);
    }
}
