using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GamePauseController : MonoBehaviour
{
    public static GamePauseController Instance { get; private set; }

    public bool IsOpen => GameStateController.Instance != null && GameStateController.Instance.IsPaused;

    private GameObject pauseRoot;
    private GameObject leaveDungeonButtonObject;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureEventSystem();
        BuildPauseUi();
    }

    private void Start()
    {
        if (GameStateController.Instance != null)
            GameStateController.Instance.StateChanged += HandleStateChanged;
    }

    private static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() != null)
            return;

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        if (InventoryUIController.Instance != null && InventoryUIController.Instance.IsOpen)
        {
            InventoryUIController.Instance.ForceClose();
            return;
        }

        GameStateController.Instance?.TogglePause();
    }

    public void ForceClose()
    {
        InventoryUIController.Instance?.ForceClose();
        GameStateController.Instance?.SetPlaying();

        if (pauseRoot != null)
            pauseRoot.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;

        if (GameStateController.Instance != null)
            GameStateController.Instance.StateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;

        if (GameStateController.Instance != null)
            GameStateController.Instance.StateChanged -= HandleStateChanged;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ForceClose();
        RefreshLeaveButtonVisibility();
    }

    private void HandleStateChanged(GameState state)
    {
        if (pauseRoot == null)
            return;

        pauseRoot.SetActive(state == GameState.Paused);
        RefreshLeaveButtonVisibility();
    }

    private void RefreshLeaveButtonVisibility()
    {
        if (leaveDungeonButtonObject != null)
            leaveDungeonButtonObject.SetActive(GameStateController.Instance?.Context == GameContext.Dungeon);
    }

    public void TogglePause()
    {
        GameStateController.Instance?.TogglePause();
    }

    public void Resume()
    {
        GameStateController.Instance?.SetPlaying();
    }

    public void ResetSave()
    {
        SaveManager.Instance?.ResetAllProgress();

        LobbyUI lobbyUI = FindFirstObjectByType<LobbyUI>();
        lobbyUI?.RefreshStatus();
    }

    public void LeaveDungeon()
    {
        if (!GameSceneNames.IsDungeonScene())
            return;

        ForceClose();
        DungeonManager.Instance?.ForceLeaveDungeon();
    }

    public void QuitGame()
    {
        GameStateController.Instance?.SetPlaying();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void BuildPauseUi()
    {
        pauseRoot = new GameObject("PauseMenuRoot");
        pauseRoot.transform.SetParent(transform, false);

        Canvas canvas = pauseRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        pauseRoot.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        pauseRoot.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f, 1080f);
        pauseRoot.AddComponent<GraphicRaycaster>();

        GameObject dim = CreateUiObject("Dim", pauseRoot.transform);
        StretchFull(dim);
        Image dimImage = dim.AddComponent<Image>();
        dimImage.color = new Color(0f, 0f, 0f, 0.65f);

        GameObject panel = CreateUiObject("Panel", pauseRoot.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(420f, 420f);
        panel.AddComponent<Image>().color = new Color(0.12f, 0.14f, 0.18f, 0.95f);

        CreateTitle(panel.transform, "Settings");
        CreateButton(panel.transform, "ResumeButton", "Resume (ESC)", 120f, Resume);
        leaveDungeonButtonObject = CreateButton(panel.transform, "LeaveDungeonButton", "Leave Dungeon", 40f, LeaveDungeon);
        CreateButton(panel.transform, "ResetSaveButton", "Reset Save", -40f, ResetSave);
        CreateButton(panel.transform, "QuitButton", "Quit Game", -120f, QuitGame);

        pauseRoot.SetActive(false);
    }

    private static GameObject CreateUiObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        return obj;
    }

    private static void StretchFull(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void CreateTitle(Transform parent, string title)
    {
        GameObject titleObject = CreateUiObject("Title", parent);
        RectTransform rect = titleObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -24f);
        rect.sizeDelta = new Vector2(360f, 48f);

        TMP_Text text = titleObject.AddComponent<TextMeshProUGUI>();
        text.text = title;
        text.fontSize = 32f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
    }

    private static GameObject CreateButton(Transform parent, string name, string label, float y, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = CreateUiObject(name, parent);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, y);
        rect.sizeDelta = new Vector2(320f, 52f);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.24f, 0.28f, 0.34f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        GameObject labelObject = CreateUiObject("Label", buttonObject.transform);
        StretchFull(labelObject);
        TMP_Text text = labelObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 24f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        return buttonObject;
    }
}
