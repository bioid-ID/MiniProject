using UnityEngine;

/// <summary>
/// Applies EnemyData sprites to body / melee swing / projectile.
/// </summary>
public static class EnemyVisualUtility
{
    public static void ApplyBody(Enemy enemy, EnemyData data, string resourcesKey = null)
    {
        if (enemy == null)
            return;

        SpriteRenderer renderer = enemy.GetComponent<SpriteRenderer>();
        if (renderer == null)
            return;

        Sprite sprite = null;
        Color tint = Color.white;

        if (data != null)
        {
            sprite = data.bodySprite;
            tint = data.bodyTint;
        }

        if (sprite == null && !string.IsNullOrEmpty(resourcesKey))
            sprite = Resources.Load<Sprite>($"Enemies/{resourcesKey}");

        if (sprite != null)
            renderer.sprite = sprite;

        renderer.color = tint;
    }

    public static void EnsureMeleeSprite(Hitbox hitbox, Sprite swingSprite)
    {
        if (hitbox == null)
            return;

        SpriteRenderer renderer = hitbox.GetComponent<SpriteRenderer>();
        if (renderer == null)
            renderer = hitbox.gameObject.AddComponent<SpriteRenderer>();

        renderer.sortingOrder = 5;
        renderer.sprite = swingSprite;
        renderer.enabled = swingSprite != null;
    }

    public static void ApplyProjectileSprite(Projectile projectile, Sprite sprite)
    {
        if (projectile == null || sprite == null)
            return;

        SpriteRenderer renderer = projectile.GetComponent<SpriteRenderer>();
        if (renderer == null)
            return;

        renderer.sprite = sprite;
    }
}
