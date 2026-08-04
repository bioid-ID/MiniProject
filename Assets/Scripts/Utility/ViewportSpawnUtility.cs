using UnityEngine;

public static class ViewportSpawnUtility
{
    public static Vector3 GetRandomEdgePositionAround(Transform center, Camera camera, float margin = 1.2f)
    {
        if (center == null)
            return Vector3.zero;

        if (camera == null)
            camera = Camera.main;

        if (camera == null || !camera.orthographic)
        {
            Vector2 fallbackOffset = Random.insideUnitCircle.normalized * 6f;
            return new Vector3(
                center.position.x + fallbackOffset.x,
                center.position.y + fallbackOffset.y,
                center.position.z);
        }

        float halfHeight = camera.orthographicSize;
        float halfWidth = halfHeight * camera.aspect;
        int edge = Random.Range(0, 4);

        Vector2 offset = edge switch
        {
            0 => new Vector2(Random.Range(-halfWidth, halfWidth), halfHeight + margin),
            1 => new Vector2(Random.Range(-halfWidth, halfWidth), -(halfHeight + margin)),
            2 => new Vector2(-(halfWidth + margin), Random.Range(-halfHeight, halfHeight)),
            _ => new Vector2(halfWidth + margin, Random.Range(-halfHeight, halfHeight))
        };

        Vector2 spawn = (Vector2)center.position + offset;
        return new Vector3(spawn.x, spawn.y, center.position.z);
    }
}
