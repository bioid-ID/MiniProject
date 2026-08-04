using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-900)]
public class NexusBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInitialize()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (!GameSceneNames.IsHubScene(sceneName))
            return;

        if (FindFirstObjectByType<NexusBootstrap>() != null)
            return;

        GameObject bootstrap = new GameObject(nameof(NexusBootstrap));
        bootstrap.AddComponent<NexusBootstrap>();
    }

    private void Awake()
    {
        HubSceneSetupUtility.Apply();
    }

    private void Start()
    {
        PortalSpawner.SpawnHubPortals(GameContentProvider.Portals);
    }
}
