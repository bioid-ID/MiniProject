using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSessionReset
{
    public static void ApplyForScene(Scene scene)
    {
        GameStateController.Instance?.PrepareSceneTransition();
        GamePauseController.Instance?.ForceClose();
        InventoryUIController.Instance?.ForceClose();

        Time.timeScale = 1f;

        if (GameSceneNames.IsHubScene(scene.name))
            ValidateHubPlayer();
        else if (GameSceneNames.IsDungeonScene(scene.name))
            ValidateDungeonPlayer(scene.name);

        Debug.Log($"[GameSessionReset] Scene={scene.name}, Context={GameStateController.Instance?.Context}, timeScale={Time.timeScale}");
    }

    private static void ValidateHubPlayer()
    {
        GameObject player = PlayerSpawnUtility.FindExistingPlayer();
        if (player == null)
        {
            Debug.LogWarning("[GameSessionReset] Hub player missing — bootstraps will spawn one.");
            return;
        }

        Debug.Log($"[GameSessionReset] Hub player OK at {player.transform.position}");
    }

    private static void ValidateDungeonPlayer(string sceneName)
    {
        GameObject player = PlayerSpawnUtility.FindExistingPlayer();
        if (player == null)
        {
            Debug.LogWarning($"[GameSessionReset] Dungeon player missing in {sceneName} — bootstraps will spawn one.");
            return;
        }

        Debug.Log($"[GameSessionReset] Dungeon player OK at {player.transform.position}");
    }
}
