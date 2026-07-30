using UnityEngine;

[CreateAssetMenu(menuName = "Loot/Loot")]
public class LootData : ScriptableObject
{
    public Sprite icon;

    public string itemName;

    public LootType lootType;

    public EquipmentData equipment;

    public SkillData skill;

    public int amount;
}