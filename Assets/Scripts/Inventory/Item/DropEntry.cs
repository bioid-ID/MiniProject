using System;

[Serializable]
public class DropEntry
{
    public ItemData item;

    [Range(0, 100)]
    public float chance = 100;

    public int minAmount = 1;

    public int maxAmount = 1;
}