using UnityEngine;

public static class DefaultItemDefinitions
{
    private static CurrencyData goldCoin;
    private static ConsumableData healthPotion;

    public static CurrencyData GoldCoin => GetOrCreateGoldCoin();
    public static ConsumableData HealthPotion => GetOrCreateHealthPotion();

    private static CurrencyData GetOrCreateGoldCoin()
    {
        if (goldCoin != null)
            return goldCoin;

        goldCoin = ScriptableObject.CreateInstance<CurrencyData>();
        goldCoin.id = 1001;
        goldCoin.itemName = "Gold Coin";
        goldCoin.description = "Currency used in shops.";
        goldCoin.itemType = ItemType.Currency;
        goldCoin.stackable = true;
        goldCoin.maxStack = 9999;
        goldCoin.sellPrice = 1;
        return goldCoin;
    }

    private static ConsumableData GetOrCreateHealthPotion()
    {
        if (healthPotion != null)
            return healthPotion;

        healthPotion = ScriptableObject.CreateInstance<ConsumableData>();
        healthPotion.id = 2001;
        healthPotion.itemName = "Health Potion";
        healthPotion.description = "Restores 30 HP. Press [U] in dungeon.";
        healthPotion.itemType = ItemType.Consumable;
        healthPotion.stackable = true;
        healthPotion.maxStack = 99;
        healthPotion.sellPrice = 25;
        healthPotion.hp = 30f;
        return healthPotion;
    }
}
