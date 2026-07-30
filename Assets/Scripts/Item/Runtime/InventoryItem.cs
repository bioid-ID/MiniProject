using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public ItemData data;

    public int quantity;

    public int enhance;

    public bool locked;

    public long uniqueID;

    public InventoryItem(ItemData data, int amount)
    {
        this.data = data;

        quantity = amount;

        uniqueID = DateTime.Now.Ticks;
    }
}