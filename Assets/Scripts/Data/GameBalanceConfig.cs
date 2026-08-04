using UnityEngine;

/// <summary>
/// BALANCE — all frequently tuned numbers live here.
/// Search: BALANCE
/// Asset: Resources/GameBalance (create via Tools > Portal Dungeon > Create Or Select GameBalance)
/// </summary>
[CreateAssetMenu(fileName = "GameBalance", menuName = "Portal Dungeon/Game Balance")]
public class GameBalanceConfig : ScriptableObject
{
    [Header("BALANCE / Portal")]
    [Tooltip("Portal interact distance")]
    public float portalInteractRange = 1.15f;

    [Tooltip("Player interact scan radius")]
    public float playerInteractRadius = 1.4f;

    [Header("BALANCE / Spawner")]
    public float spawnIntervalMain = 3f;
    public float spawnIntervalRoguelike = 2.5f;
    public int initialSpawnMain = 2;
    public int initialSpawnRoguelike = 3;
    public float viewportSpawnMargin = 1.2f;

    [Header("BALANCE / Enemy AI")]
    public float enemyDetectRange = 18f;
    public float enemyAttackRange = 1.35f;
    public float enemyMoveSpeedFallback = 1.5f;
    public float enemyStopDistanceFactor = 0.73f;

    [Header("BALANCE / Boss Wave")]
    public int bossKillsMain = 15;
    public int bossKillsRoguelike = 30;
    public float bossSecondsMain = 90f;
    public float bossSecondsRoguelike = 75f;
    public int bossStageLevelMain = 5;
    public int bossStageLevelRoguelike = 4;
    public float bossScale = 1.6f;

    [Header("BALANCE / Boss Attack")]
    public int bossComboHits = 3;
    public float bossComboGap = 0.2f;
    public float bossAttackCooldown = 1.35f;
    public float normalAttackCooldown = 1f;
    public float meleeHitDuration = 0.15f;

    [Header("BALANCE / Player Combat Feel")]
    public float meleeKnockback = 2.4f;
    public float projectileKnockback = 1.2f;
    public float meleeStun = 0.18f;
    public float projectileStun = 0.08f;
    public float critKnockbackMult = 1.35f;
    public float critStunMult = 1.5f;

    [Header("BALANCE / Regen & Steal (base)")]
    public float baseHpRegen = 1.5f;
    public float baseMpRegen = 1.0f;
    public float hpRegenPerLevel = 0.15f;
    public float mpRegenPerLevel = 0.12f;
    public float hpRegenPerStr = 0.05f;
    public float mpRegenPerInt = 0.08f;
    public float baseLifeSteal = 0.01f;
    public float baseManaSteal = 0.01f;

    [Header("BALANCE / Economy")]
    public int goldPerKill = 50;
    public float expPerKill = 20f;

    [Header("BALANCE / Roguelike Starter")]
    public int roguelikeBaseStr = 10;
    public int roguelikeBaseDex = 10;
    public int roguelikeBaseInt = 10;
    public int roguelikeBaseLuck = 10;
}

public static class GameBalance
{
    private const string ResourcePath = "GameBalance";
    private static GameBalanceConfig cached;

    public static GameBalanceConfig Config
    {
        get
        {
            if (cached != null)
                return cached;

            cached = Resources.Load<GameBalanceConfig>(ResourcePath);
            if (cached == null)
            {
                cached = ScriptableObject.CreateInstance<GameBalanceConfig>();
                Debug.LogWarning(
                    "[BALANCE] Resources/GameBalance missing — using runtime defaults. " +
                    "Unity: Tools > Portal Dungeon > Create Or Select GameBalance");
            }

            return cached;
        }
    }

    public static void ResetCache() => cached = null;

    // Short accessors for call sites
    public static float PortalInteractRange => Config.portalInteractRange;
    public static float PlayerInteractRadius => Config.playerInteractRadius;
    public static float GoldPerKill => Config.goldPerKill;
    public static float ExpPerKill => Config.expPerKill;
}
