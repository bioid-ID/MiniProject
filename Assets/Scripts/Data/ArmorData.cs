using UnityEngine;

[CreateAssetMenu(menuName = "Data/Armor")]
public class ArmorData : EquipmentData
{
    [Header("Armor")]

    public float armor;

    public float damageReduction;
}