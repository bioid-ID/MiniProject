using UnityEngine;

public class Loot : PoolObject
{
    private LootData data;

    public void Initialize(LootData loot)
    {
        data = loot;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Inventory.Instance.Add(data);

        PoolManager.Instance.Return(this);
    }
}