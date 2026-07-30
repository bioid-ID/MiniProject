using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private readonly List<InventoryItem> items = new();

    public IReadOnlyList<InventoryItem> Items => items;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        InventoryItem find =
            items.Find(x => x.item == item);

        if (find != null)
        {
            find.amount += amount;
            return;
        }

        items.Add(new InventoryItem(item, amount));
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        InventoryItem find =
            items.Find(x => x.item == item);

        if (find == null)
            return false;

        find.amount -= amount;

        if (find.amount <= 0)
            items.Remove(find);

        return true;
    }

    public bool Contains(ItemData item)
    {
        return items.Exists(x => x.item == item);
    }

    public int Count(ItemData item)
    {
        InventoryItem find =
            items.Find(x => x.item == item);

        return find == null ? 0 : find.amount;
    }
}