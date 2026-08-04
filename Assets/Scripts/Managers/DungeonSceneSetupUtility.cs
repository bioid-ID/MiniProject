using UnityEngine;
using UnityEngine.SceneManagement;

public static class DungeonSceneSetupUtility
{
    private static bool gameplayReady;

    public static void ResetGameplayState()
    {
        gameplayReady = false;
    }

    public static void EnsureCore()
    {
        DungeonSceneSanitizer.SanitizeSceneOnce();

        PoolManager poolManager = EnsurePoolManager();
        EnsurePools(poolManager);
        EnsureManagers();
    }

    public static void EnsureGameplay()
    {
        EnsureCore();

        if (gameplayReady)
            return;

        GameObject player = PlayerSpawnUtility.EnsurePlayer(PlayerSetupMode.Dungeon, Vector3.zero);
        if (player == null)
        {
            Debug.LogError("[DungeonSetup] Failed to ensure dungeon player.");
            return;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        if (player.scene != activeScene)
            SceneManager.MoveGameObjectToScene(player, activeScene);

        PlayableDungeonBootstrap.ConfigureDungeonCombat(player);
        DungeonManager.Instance?.ApplyRunModeToPlayer();

        EnsureSpawner();
        EnsureBossWave();
        EnsureReturnPortal(player.transform.position);
        EnsureDungeonInteractionUI();
        FixDungeonHudLayout();
        FixDungeonCamera();

        gameplayReady = true;

        int enemyCount = EnemyManager.Instance != null ? EnemyManager.Instance.ActiveCount : 0;
        int portalCount = Object.FindObjectsByType<PortalTrigger>(FindObjectsSortMode.None).Length;
        Debug.Log($"[DungeonSetup] Ready — enemies={enemyCount}, portals={portalCount}, poolEnemy={PoolManager.Instance != null && PoolManager.Instance.IsRegistered<Enemy>()}");
    }

    private static PoolManager EnsurePoolManager()
    {
        PoolManager keeper = PoolManager.Instance;

        if (keeper == null)
        {
            GameObject poolObject = GameObject.Find("PoolManager") ?? new GameObject("PoolManager");
            keeper = poolObject.GetComponent<PoolManager>() ?? poolObject.AddComponent<PoolManager>();
        }

        PoolManager[] poolManagers = Object.FindObjectsByType<PoolManager>(FindObjectsSortMode.None);
        foreach (PoolManager candidate in poolManagers)
        {
            if (candidate != null && candidate != keeper)
                Object.Destroy(candidate.gameObject);
        }

        return keeper;
    }

    private static void EnsurePools(PoolManager poolManager)
    {
        if (poolManager == null)
            return;

        Transform poolRoot = poolManager.transform;

        if (PoolManager.Instance == null || !PoolManager.Instance.IsRegistered<Enemy>())
        {
            Enemy enemyPrefab = CreateEnemyTemplate(poolRoot);
            poolManager.RegisterPool(enemyPrefab, 40, poolRoot);
        }

        if (PoolManager.Instance == null || !PoolManager.Instance.IsRegistered<Projectile>())
        {
            Projectile projectilePrefab = CreateProjectileTemplate(poolRoot);
            poolManager.RegisterPool(projectilePrefab, 80, poolRoot);
        }

        if (PoolManager.Instance == null || !PoolManager.Instance.IsRegistered<Loot>())
        {
            Loot lootPrefab = CreateLootTemplate(poolRoot);
            poolManager.RegisterPool(lootPrefab, 30, poolRoot);
        }
    }

    private static void EnsureManagers()
    {
        EnsureManager<EnemyManager>("EnemyManager");
        EnsureManager<DropManager>("DropManager");
        EnsureManager<BuffManager>("BuffManager");
        EnsureManager<SkillManager>("SkillManager");
        EnsureManager<DamagePopupManager>("DamagePopupManager");

        if (Object.FindFirstObjectByType<HUDController>() == null)
            EnsureManager<HUDController>("HUDController");

        if (DungeonManager.Instance == null)
            new GameObject("DungeonManager").AddComponent<DungeonManager>();
    }

    private static T EnsureManager<T>(string objectName) where T : Component
    {
        T existing = Object.FindFirstObjectByType<T>();
        if (existing != null)
            return existing;

        GameObject managerObject = new GameObject(objectName);
        return managerObject.AddComponent<T>();
    }

    private static void EnsureSpawner()
    {
        EnemySpawnerManager spawner = Object.FindFirstObjectByType<EnemySpawnerManager>();
        GameObject spawnerObject;

        if (spawner == null)
        {
            spawnerObject = new GameObject("EnemySpawnerManager");
            spawner = spawnerObject.AddComponent<EnemySpawnerManager>();
        }
        else
        {
            spawnerObject = spawner.gameObject;
        }

        Transform[] spawnPoints = DungeonSceneMarkers.GetEnemySpawnPoints(spawnerObject.transform);
        DungeonSpawnProfile profile = LoadSpawnProfile();
        spawner.Configure(spawnPoints, profile, spawnImmediatelyOnDeath: true);

        int initial = profile != null ? profile.initialSpawnCount : 2;
        spawner.ForceSpawnNow(initial);
    }

    private static void EnsureBossWave()
    {
        BossWaveController bossWave = Object.FindFirstObjectByType<BossWaveController>();
        if (bossWave == null)
            bossWave = new GameObject("BossWaveController").AddComponent<BossWaveController>();

        bossWave.Configure(LoadSpawnProfile());
    }

    private static DungeonSpawnProfile LoadSpawnProfile()
    {
        string scene = SceneManager.GetActiveScene().name;
        string key = GameSceneNames.IsRoguelikeDungeonScene(scene)
            ? "SpawnProfiles/RoguelikeDungeon"
            : "SpawnProfiles/MainDungeon";

        DungeonSpawnProfile profile = Resources.Load<DungeonSpawnProfile>(key);
        if (profile == null)
        {
            Debug.LogWarning(
                $"[Spawn] Resources/{key} missing. " +
                "Tools > Portal Dungeon > Create Default Spawn Profiles");
        }

        return profile;
    }

    private static void EnsureReturnPortal(Vector3 playerPosition)
    {
        Vector3 portalPosition = DungeonSceneMarkers.GetReturnPortalPosition(playerPosition);
        PortalSpawner.EnsureReturnPortal(portalPosition);
    }

    private static void EnsureDungeonInteractionUI()
    {
        if (InteractionPromptUI.Instance != null)
            return;

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return;

        GameObject promptObject = new GameObject("InteractionPrompt");
        promptObject.transform.SetParent(canvas.transform, false);

        RectTransform rect = promptObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 48f);
        rect.sizeDelta = new Vector2(900f, 56f);

        TMPro.TMP_Text promptText = promptObject.AddComponent<TMPro.TextMeshProUGUI>();
        promptText.alignment = TMPro.TextAlignmentOptions.Center;
        promptText.fontSize = 26f;
        promptText.color = Color.white;

        InteractionPromptUI promptUI = promptObject.AddComponent<InteractionPromptUI>();
        promptUI.BindPromptText(promptText);
    }

    private static void FixDungeonHudLayout()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return;

        DungeonUiLayoutHelper.Apply(canvas);
    }

    private static void FixDungeonCamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
            return;

        camera.orthographic = true;
        if (camera.orthographicSize < 2f)
            camera.orthographicSize = 5f;

        SimpleCameraFollow follow = camera.GetComponent<SimpleCameraFollow>();
        if (follow == null)
            follow = camera.gameObject.AddComponent<SimpleCameraFollow>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            follow.SetTarget(player.transform, snapImmediate: true);
    }

    private static Enemy CreateEnemyTemplate(Transform parent)
    {
        GameObject enemyObject = new GameObject("EnemyTemplate");
        enemyObject.SetActive(false);
        enemyObject.transform.SetParent(parent, false);

        Rigidbody2D rb = enemyObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        CircleCollider2D bodyCollider = enemyObject.AddComponent<CircleCollider2D>();
        bodyCollider.isTrigger = true;
        bodyCollider.radius = 0.4f;

        SpriteRenderer bodyRenderer = enemyObject.AddComponent<SpriteRenderer>();
        bodyRenderer.sprite = ProceduralSpriteFactory.CreateCircle(new Color(0.85f, 0.2f, 0.2f));
        bodyRenderer.sortingOrder = 1;

        enemyObject.AddComponent<EnemyMovement>();
        enemyObject.AddComponent<EnemyAttack>();
        enemyObject.AddComponent<EnemyHealth>();
        Enemy enemy = enemyObject.AddComponent<Enemy>();

        GameObject hurtboxObject = new GameObject("Hurtbox");
        hurtboxObject.transform.SetParent(enemyObject.transform, false);

        CircleCollider2D hurtboxCollider = hurtboxObject.AddComponent<CircleCollider2D>();
        hurtboxCollider.isTrigger = true;
        hurtboxCollider.radius = 0.45f;

        Hurtbox hurtbox = hurtboxObject.AddComponent<Hurtbox>();
        SetPrivateField(hurtbox, "owner", enemyObject);
        SetPrivateField(hurtbox, "team", TeamType.Enemy);

        GameObject meleeHitboxObject = new GameObject("MeleeHitbox");
        meleeHitboxObject.transform.SetParent(enemyObject.transform, false);
        meleeHitboxObject.SetActive(false);

        BoxCollider2D meleeCollider = meleeHitboxObject.AddComponent<BoxCollider2D>();
        meleeCollider.isTrigger = true;
        meleeCollider.size = new Vector2(1.2f, 1.2f);

        SpriteRenderer swingRenderer = meleeHitboxObject.AddComponent<SpriteRenderer>();
        swingRenderer.sortingOrder = 5;
        swingRenderer.enabled = false;

        Hitbox meleeHitbox = meleeHitboxObject.AddComponent<Hitbox>();
        EnemyAttack enemyAttack = enemyObject.GetComponent<EnemyAttack>();
        SetPrivateField(enemyAttack, "meleeHitbox", meleeHitbox);
        SetPrivateField(enemyAttack, "attackPoint", enemyObject.transform);

        return enemy;
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        if (target == null)
            return;

        System.Reflection.FieldInfo field = target.GetType().GetField(
            fieldName,
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Public);

        field?.SetValue(target, value);
    }

    private static Projectile CreateProjectileTemplate(Transform parent)
    {
        GameObject projectileObject = new GameObject("ProjectileTemplate");
        projectileObject.SetActive(false);
        projectileObject.transform.SetParent(parent, false);

        Rigidbody2D rb = projectileObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        CircleCollider2D collider = projectileObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.15f;

        SpriteRenderer renderer = projectileObject.AddComponent<SpriteRenderer>();
        renderer.sprite = ProceduralSpriteFactory.CreateCircle(new Color(1f, 0.9f, 0.2f));
        renderer.sortingOrder = 2;

        return projectileObject.AddComponent<Projectile>();
    }

    private static Loot CreateLootTemplate(Transform parent)
    {
        GameObject lootObject = new GameObject("LootTemplate");
        lootObject.SetActive(false);
        lootObject.transform.SetParent(parent, false);

        Rigidbody2D rb = lootObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.linearDamping = 4f;
        rb.angularDamping = 1.5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        CircleCollider2D collider = lootObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.25f;

        SpriteRenderer renderer = lootObject.AddComponent<SpriteRenderer>();
        renderer.sprite = ProceduralSpriteFactory.CreateCircle(new Color(1f, 0.85f, 0.2f));
        renderer.sortingOrder = 3;

        lootObject.AddComponent<LootPhysicsBehavior>();
        return lootObject.AddComponent<Loot>();
    }
}
