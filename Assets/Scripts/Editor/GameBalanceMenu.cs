#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Create balance / spawn data assets.
/// </summary>
public static class GameBalanceMenu
{
    private const string ResourcesDir = "Assets/Resources";
    private const string BalancePath = "Assets/Resources/GameBalance.asset";
    private const string SpawnDir = "Assets/Resources/SpawnProfiles";
    private const string EnemyDataDir = "Assets/ScriptableObjects/Enemies";

    [MenuItem("Tools/Portal Dungeon/Create Or Select GameBalance")]
    public static void CreateOrSelectBalance()
    {
        EnsureFolder(ResourcesDir);
        GameBalanceConfig existing = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(BalancePath);
        if (existing == null)
        {
            existing = ScriptableObject.CreateInstance<GameBalanceConfig>();
            AssetDatabase.CreateAsset(existing, BalancePath);
            AssetDatabase.SaveAssets();
        }

        GameBalance.ResetCache();
        Selection.activeObject = existing;
        EditorGUIUtility.PingObject(existing);
    }

    [MenuItem("Tools/Portal Dungeon/Create Default Spawn Profiles")]
    public static void CreateDefaultSpawnProfiles()
    {
        EnsureFolder(ResourcesDir);
        EnsureFolder(SpawnDir);
        EnsureFolder("Assets/ScriptableObjects");
        EnsureFolder(EnemyDataDir);

        EnemyData basic = LoadOrCreateEnemy(EnemyDataDir + "/Enemy_Basic.asset", "Basic Slime", false, gold: 40, exp: 15f, detect: 7f, attack: 1.4f);
        EnemyData elite = LoadOrCreateEnemy(EnemyDataDir + "/Enemy_Elite.asset", "Elite Brute", false, gold: 90, exp: 35f, detect: 10f, attack: 1.8f, hp: 180f, atk: 16f);
        EnemyData boss = LoadOrCreateEnemy(EnemyDataDir + "/Enemy_Boss.asset", "Dungeon Boss", true, gold: 300, exp: 120f, detect: 14f, attack: 2.2f, hp: 800f, atk: 28f);

        CreateSpawnProfile(
            SpawnDir + "/MainDungeon.asset",
            initial: 2,
            bossKills: 12,
            bossSeconds: 90f,
            bossLevel: 5,
            boss,
            new EnemySpawnEntry { prefabKey = EnemyPrefabCatalog.Basic, enemyData = basic, weight = 3f, spawnInterval = 3f, stageLevel = 1 },
            new EnemySpawnEntry { prefabKey = EnemyPrefabCatalog.Elite, enemyData = elite, weight = 1f, spawnInterval = 7f, stageLevel = 2 });

        CreateSpawnProfile(
            SpawnDir + "/RoguelikeDungeon.asset",
            initial: 3,
            bossKills: 10,
            bossSeconds: 75f,
            bossLevel: 4,
            boss,
            new EnemySpawnEntry { prefabKey = EnemyPrefabCatalog.Basic, enemyData = basic, weight = 2f, spawnInterval = 2.5f, stageLevel = 1 },
            new EnemySpawnEntry { prefabKey = EnemyPrefabCatalog.Elite, enemyData = elite, weight = 1.5f, spawnInterval = 5f, stageLevel = 2 });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Object main = AssetDatabase.LoadAssetAtPath<Object>(SpawnDir + "/MainDungeon.asset");
        Selection.activeObject = main;
        EditorGUIUtility.PingObject(main);
        Debug.Log("[Spawn] Created default EnemyData + SpawnProfiles. Tune each asset in Inspector.");
    }

    private static EnemyData LoadOrCreateEnemy(
        string path,
        string name,
        bool isBoss,
        int gold,
        float exp,
        float detect,
        float attack,
        float hp = 100f,
        float atk = 10f)
    {
        EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>(path);
        if (data != null)
            return data;

        data = ScriptableObject.CreateInstance<EnemyData>();
        data.enemyName = name;
        data.isBoss = isBoss;
        data.baseHp = hp;
        data.baseAttack = atk;
        data.goldReward = gold;
        data.expReward = exp;
        data.detectRange = detect;
        data.attackRange = attack;
        if (isBoss)
        {
            data.bossScale = 1.6f;
            data.bossComboHits = 3;
            data.bossComboGap = 0.2f;
            data.bossAttackCooldown = 1.35f;
            data.knockbackTakenMult = 0.35f;
            data.stunTakenMult = 0.4f;
        }

        AssetDatabase.CreateAsset(data, path);
        return data;
    }

    private static void CreateSpawnProfile(
        string path,
        int initial,
        int bossKills,
        float bossSeconds,
        int bossLevel,
        EnemyData boss,
        params EnemySpawnEntry[] entries)
    {
        DungeonSpawnProfile profile = AssetDatabase.LoadAssetAtPath<DungeonSpawnProfile>(path);
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<DungeonSpawnProfile>();
            AssetDatabase.CreateAsset(profile, path);
        }

        profile.initialSpawnCount = initial;
        profile.bossKillsRequired = bossKills;
        profile.bossSecondsRequired = bossSeconds;
        profile.bossStageLevel = bossLevel;
        profile.bossData = boss;
        profile.bossPrefabKey = EnemyPrefabCatalog.Boss;
        profile.enemies = new System.Collections.Generic.List<EnemySpawnEntry>(entries);
        EditorUtility.SetDirty(profile);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
        string name = Path.GetFileName(path);
        if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            EnsureFolder(parent);

        if (!string.IsNullOrEmpty(parent))
            AssetDatabase.CreateFolder(parent, name);
    }
}
#endif
