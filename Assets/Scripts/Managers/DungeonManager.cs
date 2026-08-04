using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;

    [SerializeField] private string dungeonSceneName = GameSceneNames.MainDungeon;
    [SerializeField] private string hubSceneName = GameSceneNames.Hub;

    public DungeonRunTracker RunStats { get; } = new DungeonRunTracker();
    public DungeonRunMode ActiveRunMode { get; private set; } = DungeonRunMode.None;

    public float TotalDamageDealt => RunStats.TotalDamageDealt;
    public float TotalDamageTaken => RunStats.TotalDamageTaken;
    public int KilledMonsters => RunStats.KilledMonsters;

    private bool pendingHubReturnToast;
    private int goldAtRunStart;
    private bool roguelikeActive;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void LogDamageDealt(float dmg) =>
        RunStats.LogDamageDealt(dmg, AttackMethod.Other, DamageType.Physical, false);

    public void LogDamageTaken(float dmg) => RunStats.LogDamageTaken(dmg);
    public void LogKill() => RunStats.LogKill();

    public void ResetRunStats() => RunStats.Reset();

    public bool ConsumeHubReturnToast()
    {
        if (!pendingHubReturnToast)
            return false;

        pendingHubReturnToast = false;
        return true;
    }

    public void EnterDungeon(string sceneName = null)
    {
        ResetRunStats();
        ResultUIController.Instance?.ClearLastResult();

        string targetScene = string.IsNullOrWhiteSpace(sceneName) ? dungeonSceneName : sceneName;
        targetScene = GameSceneNames.Resolve(targetScene);
        ActiveRunMode = GameSceneNames.GetRunModeForScene(targetScene);
        roguelikeActive = ActiveRunMode == DungeonRunMode.Roguelike;

        if (PlayerStat.Instance != null)
        {
            PlayerData.Instance?.SaveFrom(PlayerStat.Instance);
            SaveManager.Instance?.Save();
            goldAtRunStart = PlayerStat.Instance.Gold;
        }

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(targetScene, saveBeforeLoad: false);
            return;
        }

        SceneManager.LoadScene(targetScene);
    }

    public void ApplyRunModeToPlayer()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (GameSceneNames.IsRoguelikeDungeonScene(sceneName))
        {
            roguelikeActive = true;
            ActiveRunMode = DungeonRunMode.Roguelike;
            if (PlayerStat.Instance != null && goldAtRunStart <= 0)
                goldAtRunStart = PlayerStat.Instance.Gold;
        }

        if (!roguelikeActive || PlayerStat.Instance == null)
            return;

        PlayerStat.Instance.ApplyRoguelikeFreshStart(keepGold: true);
        PlayerStat.Instance.GetComponent<PlayerHealth>()?.ReviveFull();

        if (PlayerStat.Instance.weaponSlot == null)
            PlayerStat.Instance.EquipItem(DefaultEquipmentDefinitions.RustySword);

        Debug.Log("[DungeonManager] Roguelike fresh start applied (gold kept, stats reset).");
    }

    public void ExitOrDie()
    {
        // Gold already granted per kill from EnemyData.goldReward.
        FinalizeRunCurrency(bonusGold: 0);
        StashResultAndReturn(RunStats.BuildResult("You Died"));
    }

    public void EscapeToHub()
    {
        FinalizeRunCurrency(bonusGold: 0);
        StashResultAndReturn(RunStats.BuildResult("Escaped to Hub"));
    }

    public void EscapePortalDirect() => EscapeToHub();

    public void ForceLeaveDungeon(bool showResultScreen = false)
    {
        if (showResultScreen)
        {
            EscapeToHub();
            return;
        }

        FinalizeRunCurrency(bonusGold: 0);
        pendingHubReturnToast = true;
        DirectReturnToHub();
    }

    private void FinalizeRunCurrency(int bonusGold)
    {
        if (PlayerStat.Instance == null)
            return;

        if (roguelikeActive)
        {
            int runGoldGained = Mathf.Max(0, PlayerStat.Instance.Gold - goldAtRunStart) + Mathf.Max(0, bonusGold);

            if (PlayerData.Instance != null)
            {
                PlayerData.Instance.ApplyTo(PlayerStat.Instance);
                SaveManager.Instance?.ApplyEquipmentToCurrentPlayer();
            }

            if (runGoldGained > 0)
                PlayerStat.Instance.AddGold(runGoldGained);

            PlayerData.Instance?.SaveFrom(PlayerStat.Instance);
        }
        else
        {
            if (bonusGold > 0)
                PlayerStat.Instance.AddGold(bonusGold);

            PlayerData.Instance?.SaveFrom(PlayerStat.Instance);
        }

        SaveManager.Instance?.Save();
    }

    private void StashResultAndReturn(DungeonRunResult result)
    {
        ResultUIController.Instance?.StoreLastResult(result);
        pendingHubReturnToast = true;
        DirectReturnToHub();
    }

    private void DirectReturnToHub()
    {
        GamePauseController.Instance?.ForceClose();
        ResultUIController.Instance?.HidePanel();
        InventoryUIController.Instance?.ForceClose();

        Time.timeScale = 1f;
        roguelikeActive = false;
        ActiveRunMode = DungeonRunMode.None;
        ResetRunStats();
        ReturnToHub();
    }

    public void ReturnToHub()
    {
        string hub = GameSceneNames.Resolve(hubSceneName);

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(hub, saveBeforeLoad: false);
            return;
        }

        SceneManager.LoadScene(hub);
    }
}
