using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class PlayableDungeonBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInitialize()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (!GameSceneNames.IsDungeonScene(scene.name))
            return;

        if (FindFirstObjectByType<PlayableDungeonBootstrap>() != null)
            return;

        GameObject bootstrap = new GameObject(nameof(PlayableDungeonBootstrap));
        bootstrap.AddComponent<PlayableDungeonBootstrap>();
    }

    private void Awake()
    {
        Time.timeScale = 1f;
        GamePauseController.Instance?.ForceClose();

        EnsurePersistentData();
        PoolManager poolManager = EnsurePoolManager();
        Transform poolRoot = poolManager.transform;

        Enemy enemyPrefab = CreateEnemyTemplate(poolRoot);
        Projectile projectilePrefab = CreateProjectileTemplate(poolRoot);

        poolManager.RegisterPool(enemyPrefab, 40, poolRoot);
        poolManager.RegisterPool(projectilePrefab, 80, poolRoot);

        Loot lootPrefab = CreateLootTemplate(poolRoot);
        poolManager.RegisterPool(lootPrefab, 30, poolRoot);

        EnsureManager<EnemyManager>("EnemyManager");
        EnsureManager<DropManager>("DropManager");
        EnsureManager<BuffManager>("BuffManager");
        EnsureManager<SkillManager>("SkillManager");
        EnsureManager<DamagePopupManager>("DamagePopupManager");

        if (FindFirstObjectByType<HUDController>() == null)
            EnsureManager<HUDController>("HUDController");

        if (DungeonManager.Instance == null)
            new GameObject("DungeonManager").AddComponent<DungeonManager>();
    }

    private void Start()
    {
        SetupPlayer();
        SetupSpawner();
        EnsureReturnPortal();
        EnsureDungeonCanvas();
        EnsureDungeonInteractionUI();
        FixDungeonCamera();
    }

    private static void EnsureDungeonCanvas()
    {
        if (FindFirstObjectByType<Canvas>() != null)
            return;

        GameObject canvasObject = new GameObject("DungeonCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        UnityEngine.UI.CanvasScaler scaler = canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        canvasObject.AddComponent<HUDController>();
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

        GameObject player = PlayerSpawnUtility.FindExistingPlayer();
        if (player != null)
            follow.SetTarget(player.transform, snapImmediate: true);
    }

    private static void EnsurePersistentData()
    {
        if (PlayerData.Instance != null)
            return;

        new GameObject("PlayerData").AddComponent<PlayerData>();
    }

    private static PoolManager EnsurePoolManager()
    {
        PoolManager existing = FindFirstObjectByType<PoolManager>();

        if (existing != null)
            return existing;

        GameObject poolObject = GameObject.Find("PoolManager") ?? new GameObject("PoolManager");
        return poolObject.GetComponent<PoolManager>() ?? poolObject.AddComponent<PoolManager>();
    }

    private static T EnsureManager<T>(string objectName) where T : Component
    {
        T existing = FindFirstObjectByType<T>();

        if (existing != null)
            return existing;

        GameObject managerObject = new GameObject(objectName);
        return managerObject.AddComponent<T>();
    }

    private static Enemy CreateEnemyTemplate(Transform parent)
    {

        GameObject enemyObject = new GameObject("EnemyTemplate");
        enemyObject.SetActive(false);
        enemyObject.transform.SetParent(parent, false);
        enemyObject.layer = LayerMask.NameToLayer("Default");

        Rigidbody2D rb = enemyObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        CircleCollider2D bodyCollider = enemyObject.AddComponent<CircleCollider2D>();
        bodyCollider.isTrigger = true;   
        bodyCollider.radius = 0.4f;

        SpriteRenderer bodyRenderer = enemyObject.AddComponent<SpriteRenderer>();
        bodyRenderer.sprite = CreateCircleSprite(new Color(0.85f, 0.2f, 0.2f));
        bodyRenderer.sortingOrder = 1;

        enemyObject.AddComponent<EnemyMovement>();
        enemyObject.AddComponent<EnemyAttack>();
        enemyObject.AddComponent<EnemyHealth>();
        Enemy enemy = enemyObject.AddComponent<Enemy>();

        GameObject hurtboxObject = new GameObject("Hurtbox");
        hurtboxObject.transform.SetParent(enemyObject.transform, false);
        hurtboxObject.layer = enemyObject.layer;

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

        Hitbox meleeHitbox = meleeHitboxObject.AddComponent<Hitbox>();
        SetPrivateField(enemyObject.GetComponent<EnemyAttack>(), "meleeHitbox", meleeHitbox);
        SetPrivateField(enemyObject.GetComponent<EnemyAttack>(), "attackPoint", enemyObject.transform);

        return enemy;
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
        renderer.sprite = CreateCircleSprite(new Color(1f, 0.9f, 0.2f));
        renderer.sortingOrder = 2;

        return projectileObject.AddComponent<Projectile>();
    }

    private static Loot CreateLootTemplate(Transform parent)
    {
        GameObject lootObject = new GameObject("LootTemplate");
        lootObject.SetActive(false);
        lootObject.transform.SetParent(parent, false);

        Rigidbody2D rb = lootObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        CircleCollider2D collider = lootObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.25f;

        SpriteRenderer renderer = lootObject.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateCircleSprite(new Color(1f, 0.85f, 0.2f));
        renderer.sortingOrder = 3;

        return lootObject.AddComponent<Loot>();
    }

    private static void SetupPlayer()
    {
        GameObject playerObject = PlayerSpawnUtility.EnsurePlayer(PlayerSetupMode.Dungeon, Vector3.zero);

        if (playerObject == null)
        {
            Debug.LogError("PlayableDungeonBootstrap: Failed to ensure dungeon player.");
            return;
        }

        SetupDungeonCombat(playerObject);
    }

    private static void SetupDungeonCombat(GameObject playerObject)
    {
        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (playerObject.GetComponent<SpriteRenderer>() == null)
        {
            SpriteRenderer renderer = playerObject.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateCircleSprite(new Color(0.2f, 0.6f, 1f));
            renderer.sortingOrder = 1;
        }

        MeleeAttack meleeAttack = playerObject.GetComponent<MeleeAttack>() ?? playerObject.AddComponent<MeleeAttack>();

        Transform hitboxTransform = playerObject.transform.Find("MeleeHitbox");
        Hitbox meleeHitbox;

        if (hitboxTransform == null)
        {
            GameObject hitboxObject = new GameObject("MeleeHitbox");
            hitboxObject.transform.SetParent(playerObject.transform, false);
            hitboxObject.SetActive(false);

            BoxCollider2D hitboxCollider = hitboxObject.AddComponent<BoxCollider2D>();
            hitboxCollider.isTrigger = true;
            hitboxCollider.size = new Vector2(2.5f, 2.5f);

            meleeHitbox = hitboxObject.AddComponent<Hitbox>();
        }
        else
        {
            meleeHitbox = hitboxTransform.GetComponent<Hitbox>();
        }

        SetPrivateField(meleeAttack, "meleeHitbox", meleeHitbox);

        ProjectileAttack projectileAttack = playerObject.GetComponent<ProjectileAttack>();
        if (projectileAttack != null)
        {
            Transform firePoint = playerObject.transform.Find("FirePoint");
            if (firePoint == null)
            {
                GameObject firePointObject = new GameObject("FirePoint");
                firePointObject.transform.SetParent(playerObject.transform, false);
                firePointObject.transform.localPosition = new Vector3(0.5f, 0f, 0f);
                firePoint = firePointObject.transform;
            }

            SetPrivateField(projectileAttack, "spawnPoint", firePoint);
        }

        Transform hurtboxTransform = playerObject.transform.Find("Hurtbox");

        if (hurtboxTransform == null)
        {
            GameObject hurtboxObject = new GameObject("Hurtbox");
            hurtboxObject.transform.SetParent(playerObject.transform, false);

            CircleCollider2D hurtboxCollider = hurtboxObject.AddComponent<CircleCollider2D>();
            hurtboxCollider.isTrigger = true;
            hurtboxCollider.radius = 0.45f;

            Hurtbox hurtbox = hurtboxObject.AddComponent<Hurtbox>();
            SetPrivateField(hurtbox, "owner", playerObject);
            SetPrivateField(hurtbox, "team", TeamType.Player);
        }

        PlayerController controller = playerObject.GetComponent<PlayerController>();

        if (controller != null)
        {
            int enemyLayerMask = LayerMask.GetMask("Default");
            SetPrivateField(controller, "enemyLayer", (LayerMask)enemyLayerMask);
        }

        SetupDefaultSkills(playerObject);
        TryAutoEquipStarterWeapon(playerObject);
    }

    private static void TryAutoEquipStarterWeapon(GameObject playerObject)
    {
        PlayerStat stat = playerObject.GetComponent<PlayerStat>();
        if (stat == null || stat.weaponSlot != null)
            return;

        WeaponData starterSword = DefaultEquipmentDefinitions.RustySword;
        Inventory inventory = Inventory.Instance;
        if (inventory == null || !inventory.HasItem(starterSword, 1))
            return;

        inventory.RemoveItem(starterSword, 1);
        stat.EquipItem(starterSword);
    }

    private static void SetupDefaultSkills(GameObject playerObject)
    {
        SkillManager skillManager = FindFirstObjectByType<SkillManager>();

        if (skillManager == null)
            return;

        ProjectileSkill existingSkill = playerObject.GetComponentInChildren<ProjectileSkill>();

        if (existingSkill == null)
        {
            GameObject skillObject = new GameObject("AutoFireball");
            skillObject.transform.SetParent(playerObject.transform, false);

            existingSkill = skillObject.AddComponent<ProjectileSkill>();
            existingSkill.SetData(DefaultSkillFactory.GetFireballSkill());
        }
        else if (existingSkill.Data == null)
        {
            existingSkill.SetData(DefaultSkillFactory.GetFireballSkill());
        }

        Transform firePoint = playerObject.transform.Find("FirePoint");

        if (firePoint != null)
            SetPrivateField(existingSkill, "firePoint", firePoint);

        skillManager.AddSkill(existingSkill);
    }

    private static void SetupSpawner()
    {
        EnemySpawnerManager spawner = FindFirstObjectByType<EnemySpawnerManager>();

        if (spawner != null)
            return;

        GameObject spawnerObject = new GameObject("EnemySpawnerManager");
        spawner = spawnerObject.AddComponent<EnemySpawnerManager>();

        Transform[] spawnPoints = new Transform[4];
        Vector2[] offsets =
        {
            new Vector2(6f, 4f),
            new Vector2(-6f, 4f),
            new Vector2(6f, -4f),
            new Vector2(-6f, -4f)
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            GameObject point = new GameObject($"SpawnPoint_{i + 1}");
            point.transform.SetParent(spawnerObject.transform, false);
            point.transform.position = offsets[i];
            spawnPoints[i] = point.transform;
        }

        SetPrivateField(spawner, "spawnPoints", spawnPoints);
        SetPrivateField(spawner, "spawnInterval", 3f);
        SetPrivateField(spawner, "spawnImmediatelyOnDeath", true);
        SetPrivateField(spawner, "currentStageLevel", 1);

        spawner.SendMessage("SpawnInitialWave", SendMessageOptions.DontRequireReceiver);
    }

    private static Sprite CreateCircleSprite(Color color)
    {
        const int size = 32;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float radius = size * 0.45f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                texture.SetPixel(x, y, distance <= radius ? color : Color.clear);
            }
        }

        texture.Apply();
        texture.filterMode = FilterMode.Point;

        return Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            size);
    }

    private static void EnsureReturnPortal()
    {
        PortalSpawner.SpawnDungeonPortals(GameContentProvider.Portals);
    }

    private static void EnsureDungeonInteractionUI()
    {
        if (InteractionPromptUI.Instance != null)
            return;

        Canvas canvas = FindFirstObjectByType<Canvas>();
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
}
