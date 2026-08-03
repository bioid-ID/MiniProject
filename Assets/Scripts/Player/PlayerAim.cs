using UnityEngine;

public static class PlayerAim
{
    private static readonly Collider2D[] SearchResults = new Collider2D[32];

    public static Vector2 GetAttackDirection(Transform origin, Vector2 fallbackDirection, float searchRadius = 12f)
    {
        if (origin == null)
            return NormalizeOrRight(fallbackDirection);

        Vector2 fromMouse = GetMouseDirection(origin);
        if (fromMouse.sqrMagnitude > 0.01f)
            return fromMouse;

        Vector2 fromEnemy = GetNearestEnemyDirection(origin.position, searchRadius);
        if (fromEnemy.sqrMagnitude > 0.01f)
            return fromEnemy;

        return NormalizeOrRight(fallbackDirection);
    }

    private static Vector2 GetMouseDirection(Transform origin)
    {
        Camera camera = Camera.main;
        if (camera == null || UnityEngine.InputSystem.Mouse.current == null)
            return Vector2.zero;

        Vector3 mouseScreen = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        mouseScreen.z = Mathf.Abs(camera.transform.position.z - origin.position.z);
        Vector3 mouseWorld = camera.ScreenToWorldPoint(mouseScreen);
        Vector2 direction = (Vector2)mouseWorld - (Vector2)origin.position;

        return direction.sqrMagnitude > 0.01f ? direction.normalized : Vector2.zero;
    }

    private static Vector2 GetNearestEnemyDirection(Vector3 origin, float searchRadius)
    {
        ContactFilter2D filter = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = false
        };

        int count = Physics2D.OverlapCircle(origin, searchRadius, filter, SearchResults);
        float bestDistance = float.MaxValue;
        Vector2 bestDirection = Vector2.zero;

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = SearchResults[i];
            if (hit == null || hit.GetComponentInParent<Enemy>() == null)
                continue;

            Vector2 direction = (Vector2)hit.transform.position - (Vector2)origin;
            float distance = direction.sqrMagnitude;

            if (distance >= bestDistance)
                continue;

            bestDistance = distance;
            bestDirection = direction.normalized;
        }

        return bestDirection;
    }

    private static Vector2 NormalizeOrRight(Vector2 direction)
    {
        return direction.sqrMagnitude > 0.01f ? direction.normalized : Vector2.right;
    }

    public static void ApplyDirection(Transform target, Vector2 direction)
    {
        if (target == null)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        target.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
