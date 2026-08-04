using UnityEngine;

/// <summary>
/// 적 외형: EnemyData.bodySprite 우선, 없으면 Resources/Enemies/{이름}.
/// </summary>
public static class EnemyPrefabCatalog
{
    public const string Basic = "Enemy_Basic";
    public const string Elite = "Enemy_Elite";
    public const string Boss = "Enemy_Boss";

    public static Enemy GetFromPoolOrFallback(string resourceName)
    {
        if (PoolManager.Instance == null)
            return null;

        Enemy enemy = PoolManager.Instance.Get<Enemy>();
        if (enemy == null)
            return null;

        enemy.name = string.IsNullOrEmpty(resourceName) ? "Enemy" : resourceName;
        return enemy;
    }
}
