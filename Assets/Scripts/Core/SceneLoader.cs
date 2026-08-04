using System;
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isLoading = false;
    }

    public bool CanLoadScene(string sceneName)
    {
        return SceneBuildUtility.IsSceneInBuild(sceneName);
    }

    public void LoadScene(string sceneName, bool saveBeforeLoad = true, Action beforeLoad = null)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return;

        if (isLoading)
            Debug.LogWarning("[SceneLoader] Previous load flag was still set — forcing new load.");

        string resolvedSceneName = SceneBuildUtility.ResolveSceneName(sceneName);

        if (!SceneBuildUtility.IsSceneInBuild(resolvedSceneName))
        {
            Debug.LogError(SceneBuildUtility.GetMissingSceneHelpMessage(sceneName));
            return;
        }

        isLoading = true;
        GameStateController.Instance?.PrepareSceneTransition();
        GamePauseController.Instance?.ForceClose();
        ResultUIController.Instance?.HideImmediate();
        InventoryUIController.Instance?.ForceClose();
        Time.timeScale = 1f;

        if (SceneTransitionController.Instance != null)
        {
            SceneTransitionController.Instance.LoadScene(
                resolvedSceneName,
                saveBeforeLoad,
                beforeLoad,
                () => isLoading = false);
            return;
        }

        if (saveBeforeLoad)
            SaveManager.Instance?.Save();

        beforeLoad?.Invoke();
        Debug.Log($"[SceneLoader] Loading {resolvedSceneName}...");
        SceneManager.LoadScene(resolvedSceneName);
        isLoading = false;
    }
}
