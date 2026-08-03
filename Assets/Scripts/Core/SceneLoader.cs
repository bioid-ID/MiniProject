using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private bool isLoading;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool CanLoadScene(string sceneName)
    {
        return SceneBuildUtility.IsSceneInBuild(sceneName);
    }

    public void LoadScene(string sceneName, bool saveBeforeLoad = true)
    {
        if (isLoading || string.IsNullOrWhiteSpace(sceneName))
            return;

        if (!SceneBuildUtility.IsSceneInBuild(sceneName))
        {
            Debug.LogError(SceneBuildUtility.GetMissingSceneHelpMessage(sceneName));
            return;
        }

        isLoading = true;
        GameStateController.Instance?.PrepareSceneTransition();
        GamePauseController.Instance?.ForceClose();

        if (saveBeforeLoad)
            SaveManager.Instance?.Save();

        Debug.Log($"[SceneLoader] Loading {sceneName}...");
        SceneManager.LoadScene(sceneName);
        isLoading = false;
    }
}
