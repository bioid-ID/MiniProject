using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [Header("Info")]

    public int id;

    public string itemName;

    public Sprite icon;

    [TextArea]
    public string description;

    public ItemType itemType;

    public RarityType rarity;

    public int sellPrice;
}