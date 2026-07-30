using UnityEngine;

[System.Serializable]
public class DropEntry
{
    public LootData loot;

    [Range(0, 100)]

    public float chance;
}