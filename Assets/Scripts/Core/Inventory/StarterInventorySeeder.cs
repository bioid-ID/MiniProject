using UnityEngine;

public static class StarterInventorySeeder
{
    private static bool seeded;

    public static void Reset()
    {
        seeded = false;
    }

    public static void SeedIfEmpty()
    {
        if (seeded)
            return;

        Inventory inventory = Inventory.Instance;
        if (inventory == null)
            return;

        if (inventory.GetFilledSlotCount() > 0)
        {
            seeded = true;
            return;
        }

        ItemCatalog.EnsureDefaults();
        inventory.AddItem(DefaultItemDefinitions.GoldCoin, 10);
        inventory.AddItem(DefaultItemDefinitions.HealthPotion, 3);
        inventory.AddItem(DefaultEquipmentDefinitions.RustySword, 1);
        seeded = true;
    }
}
