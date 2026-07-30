using System;

[Serializable]
public class InventoryItem
{
    public ItemData item;

    public int amount;

    public InventoryItem(ItemData item, int amount = 1)
    {
        this.item = item;
        this.amount = amount;
    }
}