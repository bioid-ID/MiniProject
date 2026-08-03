using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;

    [SerializeField] private string dungeonSceneName = GameSceneNames.MainDungeon;
    [SerializeField] private string hubSceneName = GameSceneNames.Hub;

    public float TotalDamageDealt { get; private set; }
    public float TotalDamageTaken { get; private set; }
    public int KilledMonsters { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void LogDamageDealt(float dmg) => TotalDamageDealt += dmg;
    public void LogDamageTaken(float dmg) => TotalDamageTaken += dmg;
    public void LogKill() => KilledMonsters++;

    public void ResetRunStats()
    {
        TotalDamageDealt = 0;
        TotalDamageTaken = 0;
        KilledMonsters = 0;
    }

    public void EnterDungeon(string sceneName = null)
    {
        string targetScene = string.IsNullOrWhiteSpace(sceneName) ? dungeonSceneName : sceneName;

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(targetScene, saveBeforeLoad: true);
            return;
        }

        SaveManager.Instance?.Save();
        SceneManager.LoadScene(targetScene);
    }

    public void ExitOrDie()
    {
        int rewardGold = KilledMonsters * 50;

        if (PlayerStat.Instance != null)
        {
            PlayerStat.Instance.AddGold(rewardGold);
            PlayerData.Instance?.SaveFrom(PlayerStat.Instance);
        }

        ShowResultAndReturn(new DungeonRunResult
        {
            Title = "Run Ended",
            Kills = KilledMonsters,
            GoldEarned = rewardGold,
            DamageDealt = TotalDamageDealt,
            DamageTaken = TotalDamageTaken
        });
    }

    public void EscapeToHub()
    {
        if (PlayerStat.Instance != null)
            PlayerData.Instance?.SaveFrom(PlayerStat.Instance);

        ShowResultAndReturn(new DungeonRunResult
        {
            Title = "Escaped to Hub",
            Kills = KilledMonsters,
            GoldEarned = 0,
            DamageDealt = TotalDamageDealt,
            DamageTaken = TotalDamageTaken
        });
    }

    public void ForceLeaveDungeon()
    {
        EscapeToHub();
    }

    private void ShowResultAndReturn(DungeonRunResult result)
    {
        if (ResultUIController.Instance != null)
        {
            ResultUIController.Instance.Show(result, FinishReturnToHub);
            return;
        }

        FinishReturnToHub();
    }

    private void FinishReturnToHub()
    {
        SaveManager.Instance?.Save();
        ReturnToHub();
    }

    public void ReturnToHub()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(hubSceneName, saveBeforeLoad: false);
            return;
        }

        SceneManager.LoadScene(hubSceneName);
    }
}
