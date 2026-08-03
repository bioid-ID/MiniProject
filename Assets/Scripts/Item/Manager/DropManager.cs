using UnityEngine;

public class DropManager : MonoBehaviour
{
    public static DropManager Instance { get; private set; }

    [SerializeField] private int defaultCoinAmount = 10;
    [SerializeField] private float defaultDropChance = 100f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Drop(EnemyData enemyData, Vector3 position)
    {
        if (PoolManager.Instance == null)
            return;

        if (enemyData == null || enemyData.dropTable == null || enemyData.dropTable.Count == 0)
        {
            if (Random.value * 100f > defaultDropChance)
                return;

            SpawnLoot(DefaultLootFactory.GetCoinLoot(defaultCoinAmount), position);
            return;
        }

        foreach (DropEntry drop in enemyData.dropTable)
        {
            if (drop.loot == null)
                continue;

            if (Random.value * 100f > drop.chance)
                continue;

            SpawnLoot(drop.loot, position);
        }
    }

    private void SpawnLoot(LootData lootData, Vector3 position)
    {
        Loot loot = PoolManager.Instance.Get<Loot>();

        if (loot == null)
            return;

        Vector2 offset = Random.insideUnitCircle * 0.35f;
        loot.transform.position = position + (Vector3)offset;
        loot.Initialize(lootData);
    }
}
