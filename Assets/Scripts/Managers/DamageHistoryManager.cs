using UnityEngine;

public class DamageHistoryManager : MonoBehaviour
{
    public static DamageHistoryManager Instance;

    public float TotalDamageDealt { get; private set; }
    public float TotalDamageTaken { get; private set; }
    public int DefeatedMonsters { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else Destroy(gameObject);
    }

    public void LogDamageDealt(float amount) => TotalDamageDealt += amount;
    public void LogDamageTaken(float amount) => TotalDamageTaken += amount;
    public void LogMonsterKilled() => DefeatedMonsters++;

    public void ResetStatistics()
    {
        TotalDamageDealt = 0f;
        TotalDamageTaken = 0f;
        DefeatedMonsters = 0;
    }
}
