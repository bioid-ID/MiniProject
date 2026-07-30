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

    public ItemGrade grade;

    public bool stackable = true;

    public int maxStack = 9999;

    public int sellPrice;
}