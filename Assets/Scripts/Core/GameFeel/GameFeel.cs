using UnityEngine;

public static class GameFeel
{
    public static void PlayerHit(float damage)
    {
        CameraShake.Shake(0.12f, 0.15f);
        SoundManager.Instance?.PlaySfx(SoundManager.SfxId.PlayerHit);
        GameEvents.RaisePlayerDamaged();
    }

    public static void EnemyKilled()
    {
        SoundManager.Instance?.PlaySfx(SoundManager.SfxId.EnemyDeath);
        GameEvents.RaiseEnemyKilled();
    }

    public static void PortalEnter()
    {
        SoundManager.Instance?.PlaySfx(SoundManager.SfxId.Portal);
        GameEvents.RaisePortalUsed();
    }

    public static void PickupItem()
    {
        SoundManager.Instance?.PlaySfx(SoundManager.SfxId.Pickup);
        GameEvents.RaiseItemPickedUp();
    }

    public static void UsePotion()
    {
        SoundManager.Instance?.PlaySfx(SoundManager.SfxId.Potion);
    }

    public static void UiClick()
    {
        SoundManager.Instance?.PlaySfx(SoundManager.SfxId.UiClick);
    }
}
