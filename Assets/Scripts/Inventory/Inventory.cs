using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [SerializeField]
    private int maxSlot = 100;

    private List<InventorySlot> slots;

    private void Awake()
    {
        Instance = this;

        slots = new List<InventorySlot>();

        for (int i = 0; i < maxSlot; i++)
            slots.Add(new InventorySlot());
    }

    public bool AddItem(ItemData item, int amount)
    {
        foreach (var slot in slots)
        {
            if (slot.item == item &&
               slot.amount < item.maxStack)
            {
                slot.amount += amount;
                return true;
            }
        }

        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.item = item;
                slot.amount = amount;
                return true;
            }
        }

        return false;
    }

    public bool RemoveItem(ItemData item, int amount)
    {
        foreach (var slot in slots)
        {
            if (slot.item != item)
                continue;

            slot.amount -= amount;

            if (slot.amount <= 0)
            {
                slot.item = null;
                slot.amount = 0;
            }

            return true;
        }

        return false;
    }
}