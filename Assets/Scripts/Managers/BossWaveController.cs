using UnityEngine;

public class BossWaveController : MonoBehaviour
{
    public static BossWaveController Instance { get; private set; }

    private DungeonSpawnProfile profile;
    private int killsToBoss = 12;
    private float secondsToBoss = 90f;
    private int bossStageLevel = 5;
    private float elapsed;
    private bool bossSpawned;

    private void Awake()
    {
        Instance = this;
        elapsed = 0f;
        bossSpawned = false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        if (bossSpawned)
            return;

        if (GameStateController.Instance != null && !GameStateController.Instance.CanCombat)
            return;

        elapsed += Time.deltaTime;

        int kills = DungeonManager.Instance != null ? DungeonManager.Instance.KilledMonsters : 0;
        if (kills >= killsToBoss || elapsed >= secondsToBoss)
            SpawnBoss();
    }

    public void Configure(DungeonSpawnProfile spawnProfile)
    {
        profile = spawnProfile;
        if (profile != null)
        {
            killsToBoss = profile.bossKillsRequired;
            secondsToBoss = profile.bossSecondsRequired;
            bossStageLevel = profile.bossStageLevel;
        }

        elapsed = 0f;
        bossSpawned = false;
    }

    private void SpawnBoss()
    {
        bossSpawned = true;

        Transform player = PlayerManager.Instance != null
            ? PlayerManager.Instance.transform
            : GameObject.FindGameObjectWithTag("Player")?.transform;

        float margin = (profile != null ? profile.viewportSpawnMargin : 1.2f) + 0.8f;
        Vector3 pos = player != null
            ? ViewportSpawnUtility.GetRandomEdgePositionAround(player, Camera.main, margin)
            : Vector3.zero;

        string key = profile != null && !string.IsNullOrEmpty(profile.bossPrefabKey)
            ? profile.bossPrefabKey
            : EnemyPrefabCatalog.Boss;

        Enemy boss = EnemyPrefabCatalog.GetFromPoolOrFallback(key);
        if (boss == null)
        {
            Debug.LogWarning("[BossWave] Failed to spawn boss.");
            return;
        }

        EnemyData bossData = profile != null ? profile.bossData : null;
        if (bossData != null)
            boss.ApplyData(bossData);

        boss.transform.SetPositionAndRotation(pos, Quaternion.identity);
        boss.Initialize(bossStageLevel);

        float scale = bossData != null ? bossData.bossScale : 1.6f;
        boss.transform.localScale = Vector3.one * scale;

        SpriteRenderer renderer = boss.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.color = new Color(0.55f, 0.1f, 0.75f, 1f);

        SceneTransitionController.Instance?.ShowToast("Boss Appeared!", 2f);
        Debug.Log($"[BossWave] Boss spawned at {pos} (kills={DungeonManager.Instance?.KilledMonsters}, t={elapsed:F0}s)");
    }
}
