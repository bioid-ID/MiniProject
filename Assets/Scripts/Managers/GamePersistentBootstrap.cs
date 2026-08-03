using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePersistentBootstrap : MonoBehaviour
{
    public static GamePersistentBootstrap Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        if (FindFirstObjectByType<GamePersistentBootstrap>() != null)
            return;

        GameObject bootstrapObject = new GameObject(nameof(GamePersistentBootstrap));
        bootstrapObject.AddComponent<GamePersistentBootstrap>();
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Ensure<PlayerData>("PlayerData");
        Ensure<SaveManager>("SaveManager");
        Ensure<DungeonManager>("DungeonManager");
        Ensure<SceneLoader>("SceneLoader");
        Ensure<GameStateController>("GameStateController");
        Ensure<GamePauseController>("GamePauseController");
        Ensure<Inventory>("Inventory");
        Ensure<InventoryUIController>("InventoryUIController");
        Ensure<ConsumableUseController>("ConsumableUseController");
        Ensure<ResultUIController>("ResultUIController");
    }

    private static void Ensure<T>(string objectName) where T : Component
    {
        if (FindFirstObjectByType<T>() != null)
            return;

        new GameObject(objectName).AddComponent<T>();
    }
}
