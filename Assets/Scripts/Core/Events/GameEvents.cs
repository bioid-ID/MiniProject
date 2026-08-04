using System;

public static class GameEvents
{
    public static event Action PlayerDamaged;
    public static event Action EnemyKilled;
    public static event Action PortalUsed;
    public static event Action ItemPickedUp;
    public static event Action InventoryOpened;
    public static event Action InventoryClosed;

    public static void RaisePlayerDamaged() => PlayerDamaged?.Invoke();
    public static void RaiseEnemyKilled() => EnemyKilled?.Invoke();
    public static void RaisePortalUsed() => PortalUsed?.Invoke();
    public static void RaiseItemPickedUp() => ItemPickedUp?.Invoke();
    public static void RaiseInventoryOpened() => InventoryOpened?.Invoke();
    public static void RaiseInventoryClosed() => InventoryClosed?.Invoke();
}
