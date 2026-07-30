using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }

    [Header("Equipments")]
    public WeaponData Weapon;

    public EquipmentData Helmet;
    public EquipmentData Armor;
    public EquipmentData Pants;
    public EquipmentData Gloves;
    public EquipmentData Boots;

    public EquipmentData Necklace;

    public EquipmentData Ring1;
    public EquipmentData Ring2;

    public EquipmentData SubWeapon;

    private readonly List<EquipmentData> equipments = new();

    public IReadOnlyList<EquipmentData> Equipments => equipments;

    private void Awake()
    {
        Instance = this;

        RefreshList();
    }

    public void Equip(EquipmentData equipment)
    {
        switch (equipment.slotType)
        {
            case EquipmentSlot.Helmet:
                Helmet = equipment;
                break;

            case EquipmentSlot.Armor:
                Armor = equipment;
                break;

            case EquipmentSlot.Pants:
                Pants = equipment;
                break;

            case EquipmentSlot.Gloves:
                Gloves = equipment;
                break;

            case EquipmentSlot.Boots:
                Boots = equipment;
                break;

            case EquipmentSlot.Necklace:
                Necklace = equipment;
                break;

            case EquipmentSlot.Ring1:
                Ring1 = equipment;
                break;

            case EquipmentSlot.Ring2:
                Ring2 = equipment;
                break;

            case EquipmentSlot.SubWeapon:
                SubWeapon = equipment;
                break;
        }

        RefreshList();
    }

    public void EquipWeapon(WeaponData weapon)
    {
        Weapon = weapon;

        RefreshList();
    }

    private void RefreshList()
    {
        equipments.Clear();

        if (Weapon != null)
            equipments.Add(Weapon);

        if (Helmet != null)
            equipments.Add(Helmet);

        if (Armor != null)
            equipments.Add(Armor);

        if (Pants != null)
            equipments.Add(Pants);

        if (Gloves != null)
            equipments.Add(Gloves);

        if (Boots != null)
            equipments.Add(Boots);

        if (Necklace != null)
            equipments.Add(Necklace);

        if (Ring1 != null)
            equipments.Add(Ring1);

        if (Ring2 != null)
            equipments.Add(Ring2);

        if (SubWeapon != null)
            equipments.Add(SubWeapon);
    }
}