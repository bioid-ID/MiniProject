using UnityEngine;

public static class PlayerSpawnUtility
{
    private const string PlayerPrefabResourcePath = "Player";

    public static GameObject EnsurePlayer(PlayerSetupMode mode, Vector3 position)
    {
        GameObject playerObject = FindExistingPlayer();

        if (playerObject == null)
            playerObject = InstantiateFromPrefab() ?? CreateProceduralPlayer();

        if (playerObject == null)
            return null;

        playerObject.tag = "Player";
        playerObject.transform.position = position;
        PlayerSetupUtility.Apply(playerObject, mode);

        if (mode == PlayerSetupMode.Hub)
            PlayerSetupUtility.ResetHubTransform(playerObject);

        return playerObject;
    }

    public static GameObject FindExistingPlayer()
    {
        if (GameSceneNames.IsDungeonScene())
            return DungeonSceneSanitizer.PreparePlayer();

        GameObject tagged = GameObject.FindGameObjectWithTag("Player");
        if (tagged != null)
            return tagged;

        return GameObject.Find("Player");
    }

    private static GameObject InstantiateFromPrefab()
    {
        GameObject prefab = Resources.Load<GameObject>(PlayerPrefabResourcePath);
        if (prefab == null)
            return null;

        return Object.Instantiate(prefab);
    }

    private static GameObject CreateProceduralPlayer()
    {
        GameObject playerObject = new GameObject("Player");
        playerObject.tag = "Player";

        SpriteRenderer renderer = playerObject.AddComponent<SpriteRenderer>();
        renderer.sprite = ProceduralSpriteFactory.CreateCircle(new Color(0.25f, 0.55f, 0.95f));
        renderer.sortingOrder = 2;

        Debug.Log("PlayerSpawnUtility: Created procedural player (no Player prefab in Resources).");
        return playerObject;
    }
}
