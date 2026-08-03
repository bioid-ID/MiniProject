using System;
using UnityEngine;

public static class EquipmentSaveUtility
{
    [Serializable]
    public class SavedEquipmentSlot
    {
        public int slotType;
        public int itemId;
    }

    public static SavedEquipmentSlot[] Capture(PlayerStat stat)
    {
        if (stat == null)
            return Array.Empty<SavedEquipmentSlot>();

        return new[]
        {
            CreateSlot(EquipmentSlot.Weapon, stat.weaponSlot),
            CreateSlot(EquipmentSlot.SubWeapon, stat.subWeaponSlot),
            CreateSlot(EquipmentSlot.Helmet, stat.helmetSlot),
            CreateSlot(EquipmentSlot.Armor, stat.armorSlot),
            CreateSlot(EquipmentSlot.Pants, stat.pantsSlot),
            CreateSlot(EquipmentSlot.Gloves, stat.glovesSlot),
            CreateSlot(EquipmentSlot.Boots, stat.bootsSlot),
            CreateSlot(EquipmentSlot.Necklace, stat.necklaceSlot),
            CreateSlot(EquipmentSlot.Ring1, stat.ringSlot1),
            CreateSlot(EquipmentSlot.Ring2, stat.ringSlot2)
        };
    }

    public static void Apply(PlayerStat stat, SavedEquipmentSlot[] savedSlots)
    {
        if (stat == null)
            return;

        stat.ClearAllEquipment();

        if (savedSlots == null)
            return;

        ItemCatalog.EnsureDefaults();

        foreach (SavedEquipmentSlot savedSlot in savedSlots)
        {
            if (savedSlot == null || savedSlot.itemId <= 0)
                continue;

            if (savedSlot.slotType < 0 || savedSlot.slotType > (int)EquipmentSlot.Ring2)
                continue;

            ItemData item = ItemCatalog.GetById(savedSlot.itemId);
            EquipmentData equipment = item as EquipmentData;
            if (equipment == null)
                continue;

            stat.EquipItem(equipment);
        }
    }

    private static SavedEquipmentSlot CreateSlot(EquipmentSlot slot, EquipmentData equipment)
    {
        return new SavedEquipmentSlot
        {
            slotType = (int)slot,
            itemId = equipment != null ? equipment.id : 0
        };
    }
}
