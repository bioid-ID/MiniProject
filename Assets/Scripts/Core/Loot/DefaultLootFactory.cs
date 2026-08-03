using UnityEngine;

public static class DefaultLootFactory
{
    private static LootData defaultCoinLoot;

    public static LootData GetCoinLoot(int amount = 10)
    {
        if (defaultCoinLoot == null)
        {
            defaultCoinLoot = ScriptableObject.CreateInstance<LootData>();
            defaultCoinLoot.lootType = LootType.Coin;
            defaultCoinLoot.itemName = "Gold";
        }

        defaultCoinLoot.amount = amount;
        return defaultCoinLoot;
    }
}
