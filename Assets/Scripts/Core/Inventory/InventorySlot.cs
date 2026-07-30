using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public InventoryItem item;

    public bool IsEmpty =>
        item == null;

    public void Clear()
    {
        item = null;
    }
}