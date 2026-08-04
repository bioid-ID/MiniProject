using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSessionReset
{
    public static void ApplyForScene(Scene scene)
    {
        DungeonSceneSanitizer.ResetForNewScene();

        GameStateController.Instance?.PrepareSceneTransition();
        GamePauseController.Instance?.ForceClose();
        ResultUIController.Instance?.HidePanel();
        InventoryUIController.Instance?.ForceClose();
        StatUIController.Instance?.CloseImmediate();

        Time.timeScale = 1f;

        if (GameSceneNames.IsHubScene(scene.name))
        {
            HubSceneSetupUtility.Apply();
            PortalSpawner.SpawnHubPortals(GameContentProvider.Portals);

            PlayerHealth health = Object.FindFirstObjectByType<PlayerHealth>();
            health?.ReviveFull();

            ResultUIController.Instance?.OnReturnedToHub();

            if (DungeonManager.Instance != null && DungeonManager.Instance.ConsumeHubReturnToast())
                SceneTransitionController.Instance?.ShowToast("Returned to Nexus — open Run Report");
        }
        else if (GameSceneNames.IsDungeonScene(scene.name))
        {
            DungeonSceneSetupUtility.EnsureGameplay();
        }

        GameStateController.Instance?.NotifySceneTransitionComplete();

        Debug.Log($"[GameSessionReset] Scene={scene.name}, Context={GameStateController.Instance?.Context}, timeScale={Time.timeScale}");
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
