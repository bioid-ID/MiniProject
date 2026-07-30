using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [SerializeField]
    private int slotCount = 100;

    public List<InventorySlot> Slots { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Slots = new List<InventorySlot>();

        for (int i = 0; i < slotCount; i++)
            Slots.Add(new InventorySlot());
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null)
            return false;

        if (item.stackable)
        {
            foreach (var slot in Slots)
            {
                if (slot.IsEmpty)
                    continue;

                if (slot.item.data != item)
                    continue;

                slot.item.quantity += amount;

                return true;
            }
        }

        foreach (var slot in Slots)
        {
            if (!slot.IsEmpty)
                continue;

            slot.item = new InventoryItem(item, amount);

            return true;
        }

        return false;
    }

    public void RemoveItem(ItemData item, int amount = 1)
    {
        foreach (var slot in Slots)
        {
            if (slot.IsEmpty)
                continue;

            if (slot.item.data != item)
                continue;

            slot.item.quantity -= amount;

            if (slot.item.quantity <= 0)
                slot.Clear();

            return;
        }
    }

    public bool HasItem(ItemData item, int amount)
    {
        int count = 0;

        foreach (var slot in Slots)
        {
            if (slot.IsEmpty)
                continue;

            if (slot.item.data != item)
                continue;

            count += slot.item.quantity;
        }

        return count >= amount;
    }
}