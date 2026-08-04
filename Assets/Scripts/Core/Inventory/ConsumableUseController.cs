using UnityEngine;
using UnityEngine.InputSystem;

public class ConsumableUseController : MonoBehaviour
{
    private void Update()
    {
        if (!GameInput.WasPressed(GameAction.Potion))
            return;

        if (GameStateController.Instance == null || !GameStateController.Instance.IsPlaying)
            return;

        if (GameStateController.Instance.IsInventoryOpen)
            return;

        TryUseHealthPotion();
    }

    private static void TryUseHealthPotion()
    {
        Inventory inventory = Inventory.Instance;
        if (inventory == null)
            return;

        ConsumableData potion = DefaultItemDefinitions.HealthPotion;
        if (!inventory.HasItem(potion, 1))
            return;

        PlayerHealth health = FindPlayerHealth();
        if (health == null)
            return;

        inventory.RemoveItem(potion, 1);
        health.Heal(potion.hp, HealSource.Potion);
        GameFeel.UsePotion();
        SaveManager.Instance?.Save();
    }

    private static PlayerHealth FindPlayerHealth()
    {
        GameObject player = PlayerSpawnUtility.FindExistingPlayer();
        return player != null ? player.GetComponent<PlayerHealth>() : null;
    }
}
