using UnityEngine;

public static class DefaultEquipmentDefinitions
{
    private static WeaponData rustySword;
    private static ArmorData leatherArmor;

    public static WeaponData RustySword => GetOrCreateRustySword();
    public static ArmorData LeatherArmor => GetOrCreateLeatherArmor();

    private static WeaponData GetOrCreateRustySword()
    {
        if (rustySword != null)
            return rustySword;

        rustySword = ScriptableObject.CreateInstance<WeaponData>();
        rustySword.id = 3001;
        rustySword.itemName = "Rusty Sword";
        rustySword.equipmentName = "Rusty Sword";
        rustySword.description = "A worn blade. Better than nothing.";
        rustySword.itemType = ItemType.Equipment;
        rustySword.stackable = false;
        rustySword.maxStack = 1;
        rustySword.sellPrice = 15;
        rustySword.slotType = EquipmentSlot.Weapon;
        rustySword.requiredLevel = 1;
        rustySword.weaponType = WeaponType.Melee;
        rustySword.weaponAtk = 12f;
        rustySword.attackRange = 2f;
        rustySword.attackSpeed = 1f;
        rustySword.knockBack = 2.8f;
        rustySword.bonusStr = 2;
        rustySword.bonusLifeSteal = 0.04f;
        rustySword.bonusManaSteal = 0.02f;
        return rustySword;
    }

    private static ArmorData GetOrCreateLeatherArmor()
    {
        if (leatherArmor != null)
            return leatherArmor;

        leatherArmor = ScriptableObject.CreateInstance<ArmorData>();
        leatherArmor.id = 3002;
        leatherArmor.itemName = "Leather Armor";
        leatherArmor.equipmentName = "Leather Armor";
        leatherArmor.description = "Light protection for early runs.";
        leatherArmor.itemType = ItemType.Equipment;
        leatherArmor.stackable = false;
        leatherArmor.maxStack = 1;
        leatherArmor.sellPrice = 40;
        leatherArmor.slotType = EquipmentSlot.Armor;
        leatherArmor.requiredLevel = 1;
        leatherArmor.bonusMaxHealth = 20f;
        leatherArmor.bonusDefense = 3f;
        leatherArmor.armor = 3f;
        return leatherArmor;
    }
}
