using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;

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

    public void EnterDungeon()
    {
        TotalDamageDealt = 0; TotalDamageTaken = 0; KilledMonsters = 0;
        SceneManager.LoadScene("PortalDungeon"); 
    }

    public void ExitOrDie()
    {
        int rewardGold = KilledMonsters * 50;
        PlayerData.Instance.Gold += rewardGold;

        SceneManager.LoadScene("Lobby");
    }
}
