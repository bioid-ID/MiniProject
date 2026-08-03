using UnityEngine;

public class Loot : PoolObject
{
    private LootData data;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(LootData loot)
    {
        data = loot;
        ApplyVisual();
    }

    public override void OnSpawn()
    {
        ApplyVisual();
    }

    private void ApplyVisual()
    {
        if (spriteRenderer == null || data == null)
            return;

        switch (data.lootType)
        {
            case LootType.Coin:
                spriteRenderer.color = new Color(1f, 0.85f, 0.2f);
                break;

            case LootType.Equipment:
                spriteRenderer.color = new Color(0.4f, 0.9f, 1f);
                break;

            default:
                spriteRenderer.color = Color.white;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        ApplyLoot();
        PoolManager.Instance?.Return(this);
    }

    private void ApplyLoot()
    {
        if (data == null)
            return;

        switch (data.lootType)
        {
            case LootType.Coin:
                if (PlayerStat.Instance != null)
                    PlayerStat.Instance.AddGold(Mathf.Max(1, data.amount));
                break;

            case LootType.Equipment:
                if (data.equipment == null)
                    break;

                Inventory.Instance?.AddItem(
                    data.equipment,
                    data.amount > 0 ? data.amount : 1);

                PlayerStat.Instance?.EquipItem(data.equipment);
                break;

            default:
                Debug.Log($"Loot acquired: {data.itemName}");
                break;
        }
    }
}
