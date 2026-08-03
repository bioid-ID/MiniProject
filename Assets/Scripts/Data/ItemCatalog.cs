using System.Collections.Generic;
using UnityEngine;

public static class ItemCatalog
{
    private static readonly Dictionary<int, ItemData> ItemsById = new Dictionary<int, ItemData>();
    private static bool initialized;

    public static void Register(ItemData item)
    {
        if (item == null || item.id <= 0)
            return;

        ItemsById[item.id] = item;
    }

    public static ItemData GetById(int itemId)
    {
        EnsureDefaults();

        if (ItemsById.TryGetValue(itemId, out ItemData item))
            return item;

        return null;
    }

    public static void EnsureDefaults()
    {
        if (initialized)
            return;

        initialized = true;
        Register(DefaultItemDefinitions.GoldCoin);
        Register(DefaultItemDefinitions.HealthPotion);
        Register(DefaultEquipmentDefinitions.RustySword);
        Register(DefaultEquipmentDefinitions.LeatherArmor);
    }
}
