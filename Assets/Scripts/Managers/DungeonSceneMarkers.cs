using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Optional scene markers. Place empty GameObjects in the dungeon scene:
/// - Name "ReturnPortalMarker" for escape portal position
/// - Name "EnemySpawnMarker" (can be multiple) for enemy spawn points
/// </summary>
public static class DungeonSceneMarkers
{
    public const string ReturnPortalMarkerName = "ReturnPortalMarker";
    public const string EnemySpawnMarkerName = "EnemySpawnMarker";

    public static Vector3 GetReturnPortalPosition(Vector3 fallbackNearPlayer)
    {
        GameObject marker = GameObject.Find(ReturnPortalMarkerName);
        if (marker != null)
            return marker.transform.position;

        return fallbackNearPlayer + new Vector3(0f, 2.2f, 0f);
    }

    public static Transform[] GetEnemySpawnPoints(Transform fallbackParent)
    {
        List<Transform> spawnPoints = new List<Transform>();

        foreach (GameObject obj in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (obj != null && obj.name == EnemySpawnMarkerName)
                spawnPoints.Add(obj.transform);
        }

        if (spawnPoints.Count > 0)
            return spawnPoints.ToArray();

        return BuildDefaultSpawnPoints(fallbackParent);
    }

    private static Transform[] BuildDefaultSpawnPoints(Transform parent)
    {
        Transform[] spawnPoints = new Transform[4];
        Vector2[] offsets =
        {
            new Vector2(5f, 3f),
            new Vector2(-5f, 3f),
            new Vector2(5f, -3f),
            new Vector2(-5f, -3f)
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            Transform existing = parent.Find($"SpawnPoint_{i + 1}");
            if (existing == null)
            {
                GameObject point = new GameObject($"SpawnPoint_{i + 1}");
                point.transform.SetParent(parent, false);
                existing = point.transform;
            }

            existing.position = offsets[i];
            spawnPoints[i] = existing;
        }

        return spawnPoints;
    }
}
