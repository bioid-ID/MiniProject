using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    public static EnemySpawnerManager Instance { get; private set; }

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool spawnImmediatelyOnDeath = true;
    [SerializeField] private bool useViewportEdgeSpawn = true;

    private DungeonSpawnProfile profile;
    private readonly List<float> nextSpawnTimes = new();
    private bool periodicSpawnStarted;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Configure(Transform[] points, DungeonSpawnProfile spawnProfile, bool spawnImmediatelyOnDeath = true)
    {
        spawnPoints = points;
        profile = spawnProfile;
        this.spawnImmediatelyOnDeath = spawnImmediatelyOnDeath;

        nextSpawnTimes.Clear();
        if (profile != null && profile.enemies != null)
        {
            for (int i = 0; i < profile.enemies.Count; i++)
                nextSpawnTimes.Add(Time.time + Mathf.Max(0.1f, profile.enemies[i].spawnInterval));
        }

        StartPeriodicSpawn();
    }

    public void ForceSpawnNow(int count = 1)
    {
        int spawned = 0;
        for (int i = 0; i < count; i++)
        {
            if (TrySpawnWeighted())
                spawned++;
        }

        if (spawned == 0)
            Debug.LogWarning("[EnemySpawner] Failed to spawn. Assign DungeonSpawnProfile + EnemyData, and check PoolManager.");
        else
            Debug.Log($"[EnemySpawner] Spawned {spawned} enemy(s).");
    }

    private void StartPeriodicSpawn()
    {
        if (periodicSpawnStarted)
            return;

        periodicSpawnStarted = true;
        StartCoroutine(PeriodicSpawnRoutine());
    }

    private IEnumerator PeriodicSpawnRoutine()
    {
        while (true)
        {
            yield return null;

            if (profile == null || profile.enemies == null || profile.enemies.Count == 0)
                continue;

            float now = Time.time;
            for (int i = 0; i < profile.enemies.Count; i++)
            {
                if (i >= nextSpawnTimes.Count)
                    nextSpawnTimes.Add(now);

                if (now < nextSpawnTimes[i])
                    continue;

                EnemySpawnEntry entry = profile.enemies[i];
                if (!CanSpawnEntry(entry, i))
                {
                    nextSpawnTimes[i] = now + 0.5f;
                    continue;
                }

                if (TrySpawnEntry(entry, i))
                    nextSpawnTimes[i] = now + Mathf.Max(0.2f, entry.spawnInterval);
                else
                    nextSpawnTimes[i] = now + 0.5f;
            }
        }
    }

    public void OnMonsterKilled(Enemy deadEnemy, Vector3 deadPos)
    {
        DungeonManager.Instance?.LogKill();
        GrantRewards(deadEnemy);

        if (!spawnImmediatelyOnDeath)
            return;

        TrySpawnWeighted();
    }

    private void GrantRewards(Enemy deadEnemy)
    {
        EnemyData data = deadEnemy != null ? deadEnemy.Data : null;
        int gold = data != null ? data.goldReward : 10;
        float exp = data != null ? data.expReward : 15f;

        if (PlayerStat.Instance == null)
            return;

        if (gold > 0)
        {
            PlayerStat.Instance.AddGold(gold);
            DungeonManager.Instance?.RunStats.LogGold(gold);
        }

        if (exp > 0f)
            PlayerStat.Instance.GainExp(exp);
    }

    private bool TrySpawnWeighted()
    {
        if (profile == null || profile.enemies == null || profile.enemies.Count == 0)
            return SpawnLegacyFallback();

        if (!TryPickWeighted(profile.enemies, out EnemySpawnEntry entry, out int index))
            return false;

        return TrySpawnEntry(entry, index);
    }

    private bool CanSpawnEntry(EnemySpawnEntry entry, int entryIndex)
    {
        if (entry == null)
            return false;

        if (entry.maxAlive <= 0)
            return true;

        int alive = EnemyManager.Instance != null
            ? EnemyManager.Instance.CountAliveBySpawnEntry(entryIndex)
            : 0;

        return alive < entry.maxAlive;
    }

    private bool TrySpawnEntry(EnemySpawnEntry entry, int entryIndex)
    {
        if (entry == null || !CanSpawnEntry(entry, entryIndex))
            return false;

        Vector3 position = ResolveSpawnPosition();
        Enemy enemy = EnemyPrefabCatalog.GetFromPoolOrFallback(entry.prefabKey);
        if (enemy == null)
            return false;

        enemy.ApplyData(entry.enemyData, entry.prefabKey, entryIndex);
        enemy.transform.SetPositionAndRotation(position, Quaternion.identity);
        enemy.Initialize(entry.stageLevel > 0 ? entry.stageLevel : 1);
        return true;
    }

    private bool SpawnLegacyFallback()
    {
        if (PoolManager.Instance == null)
            return false;

        Enemy enemy = PoolManager.Instance.Get<Enemy>();
        if (enemy == null)
            return false;

        enemy.transform.SetPositionAndRotation(ResolveSpawnPosition(), Quaternion.identity);
        enemy.Initialize(1);
        return true;
    }

    private bool TryPickWeighted(List<EnemySpawnEntry> entries, out EnemySpawnEntry picked, out int pickedIndex)
    {
        picked = null;
        pickedIndex = -1;

        float total = 0f;
        for (int i = 0; i < entries.Count; i++)
        {
            EnemySpawnEntry e = entries[i];
            if (e == null || !CanSpawnEntry(e, i))
                continue;

            total += Mathf.Max(0f, e.weight);
        }

        if (total <= 0f)
            return false;

        float roll = Random.Range(0f, total);
        float acc = 0f;
        for (int i = 0; i < entries.Count; i++)
        {
            EnemySpawnEntry e = entries[i];
            if (e == null || !CanSpawnEntry(e, i))
                continue;

            acc += Mathf.Max(0f, e.weight);
            if (roll <= acc)
            {
                picked = e;
                pickedIndex = i;
                return true;
            }
        }

        return false;
    }

    private Vector3 ResolveSpawnPosition()
    {
        Transform player = PlayerManager.Instance != null
            ? PlayerManager.Instance.transform
            : GameObject.FindGameObjectWithTag("Player")?.transform;

        float margin = profile != null ? profile.viewportSpawnMargin : 1.2f;

        if (useViewportEdgeSpawn && player != null)
            return ViewportSpawnUtility.GetRandomEdgePositionAround(player, Camera.main, margin);

        if (spawnPoints == null || spawnPoints.Length == 0)
            return player != null ? player.position + (Vector3)(Random.insideUnitCircle.normalized * 6f) : Vector3.zero;

        int index = Random.Range(0, spawnPoints.Length);
        return spawnPoints[index].position;
    }
}
