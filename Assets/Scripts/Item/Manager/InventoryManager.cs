using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public IReadOnlyList<InventoryItem> Items => Inventory.Instance != null
        ? ReadItemsFromInventory()
        : System.Array.Empty<InventoryItem>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        Inventory.Instance?.AddItem(item, amount);
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (Inventory.Instance == null || !Inventory.Instance.HasItem(item, amount))
            return false;

        Inventory.Instance.RemoveItem(item, amount);
        return true;
    }

    public bool Contains(ItemData item)
    {
        return Inventory.Instance != null && Inventory.Instance.HasItem(item, 1);
    }

    public int Count(ItemData item)
    {
        if (Inventory.Instance == null)
            return 0;

        int total = 0;

        foreach (InventorySlot slot in Inventory.Instance.Slots)
        {
            if (slot.IsEmpty || slot.item.data != item)
                continue;

            total += slot.item.quantity;
        }

        return total;
    }

    private static List<InventoryItem> ReadItemsFromInventory()
    {
        List<InventoryItem> items = new List<InventoryItem>();

        foreach (InventorySlot slot in Inventory.Instance.Slots)
        {
            if (slot.IsEmpty)
                continue;

            items.Add(slot.item);
        }

        return items;
    }
}
