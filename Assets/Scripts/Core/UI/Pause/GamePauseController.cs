using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GamePauseController : MonoBehaviour
{
    private enum SettingsTab
    {
        Game,
        Audio,
        Controls,
        System
    }

    public static GamePauseController Instance { get; private set; }

    public bool IsOpen => GameStateController.Instance != null && GameStateController.Instance.IsPaused;

    private GameObject pauseRoot;
    private GameObject leaveDungeonButtonObject;
    private RectTransform contentRoot;
    private SettingsTab currentTab = SettingsTab.Game;
    private readonly System.Collections.Generic.Dictionary<SettingsTab, Button> tabButtons =
        new System.Collections.Generic.Dictionary<SettingsTab, Button>();
    private KeybindSettingsUI keybindSettings;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        UiEventSystemUtility.EnsureExists();
        keybindSettings = gameObject.AddComponent<KeybindSettingsUI>();
        BuildPauseUi();
    }

    private void Start()
    {
        if (GameStateController.Instance != null)
            GameStateController.Instance.StateChanged += HandleStateChanged;
    }

    private void Update()
    {
        if (!GameInput.WasPressed(GameAction.Pause))
            return;

        if (StatUIController.Instance != null && StatUIController.Instance.IsOpen)
        {
            StatUIController.Instance.CloseImmediate();
            return;
        }

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
        StatUIController.Instance?.CloseImmediate();
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
        if (state == GameState.Paused)
            ShowTab(currentTab);
        RefreshLeaveButtonVisibility();
    }

    private void RefreshLeaveButtonVisibility()
    {
        if (leaveDungeonButtonObject != null)
            leaveDungeonButtonObject.SetActive(GameStateController.Instance?.Context == GameContext.Dungeon);
    }

    public void Resume()
    {
        GameFeel.UiClick();
        GameStateController.Instance?.SetPlaying();
    }

    public void ResetSave()
    {
        GameFeel.UiClick();
        SaveManager.Instance?.ResetAllProgress();
        FindFirstObjectByType<LobbyUI>()?.RefreshStatus();
    }

    public void LeaveDungeon()
    {
        if (!GameSceneNames.IsDungeonScene())
            return;

        GameFeel.UiClick();
        GameStateController.Instance?.PrepareSceneTransition();
        DungeonManager.Instance?.ForceLeaveDungeon(showResultScreen: false);
    }

    public void QuitGame()
    {
        GameFeel.UiClick();
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

        CanvasScaler scaler = pauseRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        pauseRoot.AddComponent<GraphicRaycaster>();

        GameObject dim = CreateUiObject("Dim", pauseRoot.transform);
        StretchFull(dim.GetComponent<RectTransform>());
        dim.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.68f);

        GameObject panel = CreateUiObject("Panel", pauseRoot.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(980f, 620f);
        panel.AddComponent<Image>().color = new Color(0.11f, 0.13f, 0.17f, 0.97f);

        TMP_Text title = CreateText(panel.transform, "Title", "SETTINGS", 30f, TextAlignmentOptions.MidlineLeft);
        RectTransform titleRect = title.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(18f, -14f);
        titleRect.sizeDelta = new Vector2(-36f, 42f);

        CreateButton(panel.transform, "CloseButton", "Resume", new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(1f, 1f), new Vector2(-18f, -14f), new Vector2(140f, 40f), Resume);

        GameObject sidebar = CreateUiObject("Sidebar", panel.transform);
        RectTransform sidebarRect = sidebar.GetComponent<RectTransform>();
        sidebarRect.anchorMin = new Vector2(0f, 0f);
        sidebarRect.anchorMax = new Vector2(0f, 1f);
        sidebarRect.pivot = new Vector2(0f, 0.5f);
        sidebarRect.anchoredPosition = new Vector2(16f, -12f);
        sidebarRect.sizeDelta = new Vector2(180f, -80f);
        sidebar.AddComponent<Image>().color = new Color(0.08f, 0.09f, 0.12f, 0.95f);

        float tabY = -16f;
        CreateTabButton(sidebar.transform, SettingsTab.Game, "Game", ref tabY);
        CreateTabButton(sidebar.transform, SettingsTab.Audio, "Audio", ref tabY);
        CreateTabButton(sidebar.transform, SettingsTab.Controls, "Controls", ref tabY);
        CreateTabButton(sidebar.transform, SettingsTab.System, "System", ref tabY);

        GameObject content = CreateUiObject("Content", panel.transform);
        contentRoot = content.GetComponent<RectTransform>();
        contentRoot.anchorMin = new Vector2(0f, 0f);
        contentRoot.anchorMax = new Vector2(1f, 1f);
        contentRoot.offsetMin = new Vector2(212f, 20f);
        contentRoot.offsetMax = new Vector2(-20f, -64f);
        content.AddComponent<Image>().color = new Color(0.09f, 0.1f, 0.13f, 0.9f);

        ShowTab(SettingsTab.Game);
        pauseRoot.SetActive(false);
    }

    private void CreateTabButton(Transform parent, SettingsTab tab, string label, ref float y)
    {
        GameObject buttonObject = CreateUiObject($"Tab_{tab}", parent);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, y);
        rect.sizeDelta = new Vector2(156f, 44f);
        y -= 52f;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.2f, 0.24f, 0.3f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        SettingsTab captured = tab;
        button.onClick.AddListener(() =>
        {
            GameFeel.UiClick();
            ShowTab(captured);
        });

        TMP_Text text = CreateText(buttonObject.transform, "Label", label, 20f, TextAlignmentOptions.Center);
        StretchFull(text.rectTransform);

        tabButtons[tab] = button;
    }

    private void ShowTab(SettingsTab tab)
    {
        currentTab = tab;
        ClearContent();
        RefreshTabColors();

        switch (tab)
        {
            case SettingsTab.Game:
                BuildGameTab();
                break;
            case SettingsTab.Audio:
                BuildAudioTab();
                break;
            case SettingsTab.Controls:
                BuildControlsTab();
                break;
            case SettingsTab.System:
                BuildSystemTab();
                break;
        }

        RefreshLeaveButtonVisibility();
    }

    private void RefreshTabColors()
    {
        foreach (var pair in tabButtons)
        {
            Image image = pair.Value.targetGraphic as Image;
            if (image == null)
                continue;

            image.color = pair.Key == currentTab
                ? new Color(0.32f, 0.48f, 0.72f, 1f)
                : new Color(0.2f, 0.24f, 0.3f, 1f);
        }
    }

    private void BuildGameTab()
    {
        float y = -24f;
        CreateSectionTitle("Game", ref y);
        CreateBodyText("Pause menu and dungeon utilities.", ref y);
        y -= 12f;
        CreateContentButton("ResumeButton", "Resume Game", ref y, Resume);
        leaveDungeonButtonObject = CreateContentButton("LeaveDungeonButton", "Leave Dungeon", ref y, LeaveDungeon);
    }

    private void BuildAudioTab()
    {
        float y = -24f;
        CreateSectionTitle("Audio", ref y);
        CreateBodyText("Master / BGM / SFX volume.", ref y);
        y -= 8f;
        CreateVolumeSlider("MasterVolume", "Master", GameSettings.MasterVolume, value => GameSettings.MasterVolume = value, ref y);
        CreateVolumeSlider("BgmVolume", "BGM", GameSettings.BgmVolume, value => GameSettings.BgmVolume = value, ref y);
        CreateVolumeSlider("SfxVolume", "SFX", GameSettings.SfxVolume, value => GameSettings.SfxVolume = value, ref y);
    }

    private void BuildControlsTab()
    {
        float y = -24f;
        CreateSectionTitle("Controls", ref y);
        CreateBodyText("Click a key to rebind. ESC cancels rebind.", ref y);
        keybindSettings.BuildInto(contentRoot, y - 8f);
    }

    private void BuildSystemTab()
    {
        float y = -24f;
        CreateSectionTitle("System", ref y);
        CreateBodyText("Save and application options.", ref y);
        y -= 12f;
        CreateContentButton("ResetSaveButton", "Reset Save Data", ref y, ResetSave);
        CreateContentButton("QuitButton", "Quit Game", ref y, QuitGame);
    }

    private void ClearContent()
    {
        leaveDungeonButtonObject = null;
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
            Destroy(contentRoot.GetChild(i).gameObject);
    }

    private void CreateSectionTitle(string title, ref float y)
    {
        TMP_Text text = CreateText(contentRoot, "SectionTitle", title, 26f, TextAlignmentOptions.MidlineLeft);
        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(20f, y);
        rect.sizeDelta = new Vector2(-40f, 34f);
        y -= 38f;
    }

    private void CreateBodyText(string body, ref float y)
    {
        TMP_Text text = CreateText(contentRoot, "Body", body, 18f, TextAlignmentOptions.TopLeft);
        text.color = new Color(0.75f, 0.8f, 0.88f, 1f);
        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(20f, y);
        rect.sizeDelta = new Vector2(-40f, 28f);
        y -= 34f;
    }

    private GameObject CreateContentButton(string name, string label, ref float y, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = CreateUiObject(name, contentRoot);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(20f, y);
        rect.sizeDelta = new Vector2(320f, 46f);
        y -= 56f;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.24f, 0.28f, 0.34f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        TMP_Text text = CreateText(buttonObject.transform, "Label", label, 22f, TextAlignmentOptions.Center);
        StretchFull(text.rectTransform);
        return buttonObject;
    }

    private void CreateVolumeSlider(
        string name,
        string label,
        float initialValue,
        UnityEngine.Events.UnityAction<float> onChanged,
        ref float y)
    {
        GameObject row = CreateUiObject(name, contentRoot);
        RectTransform rowRect = row.GetComponent<RectTransform>();
        rowRect.anchorMin = new Vector2(0f, 1f);
        rowRect.anchorMax = new Vector2(1f, 1f);
        rowRect.pivot = new Vector2(0f, 1f);
        rowRect.anchoredPosition = new Vector2(20f, y);
        rowRect.sizeDelta = new Vector2(-40f, 48f);
        y -= 58f;

        TMP_Text labelText = CreateText(row.transform, "Label", label, 18f, TextAlignmentOptions.MidlineLeft);
        RectTransform labelRect = labelText.rectTransform;
        labelRect.anchorMin = new Vector2(0f, 0f);
        labelRect.anchorMax = new Vector2(0.22f, 1f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TMP_Text valueText = CreateText(row.transform, "Value", $"{Mathf.RoundToInt(initialValue * 100f)}%", 18f, TextAlignmentOptions.MidlineRight);
        RectTransform valueRect = valueText.rectTransform;
        valueRect.anchorMin = new Vector2(0.88f, 0f);
        valueRect.anchorMax = new Vector2(1f, 1f);
        valueRect.offsetMin = Vector2.zero;
        valueRect.offsetMax = Vector2.zero;

        GameObject sliderObject = CreateUiObject("Slider", row.transform);
        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.24f, 0.25f);
        sliderRect.anchorMax = new Vector2(0.86f, 0.75f);
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;

        Slider slider = sliderObject.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = initialValue;
        slider.onValueChanged.AddListener(value =>
        {
            onChanged(value);
            valueText.text = $"{Mathf.RoundToInt(value * 100f)}%";
        });

        GameObject background = CreateUiObject("Background", sliderObject.transform);
        StretchFull(background.GetComponent<RectTransform>());
        background.AddComponent<Image>().color = new Color(0.18f, 0.2f, 0.24f, 1f);

        GameObject fillArea = CreateUiObject("Fill Area", sliderObject.transform);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        StretchFull(fillAreaRect);
        fillAreaRect.offsetMin = new Vector2(8f, 0f);
        fillAreaRect.offsetMax = new Vector2(-8f, 0f);

        GameObject fill = CreateUiObject("Fill", fillArea.transform);
        StretchFull(fill.GetComponent<RectTransform>());
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.35f, 0.75f, 1f, 1f);

        GameObject handleSlideArea = CreateUiObject("Handle Slide Area", sliderObject.transform);
        StretchFull(handleSlideArea.GetComponent<RectTransform>());

        GameObject handle = CreateUiObject("Handle", handleSlideArea.transform);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(18f, 18f);
        handle.AddComponent<Image>().color = Color.white;

        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = handleRect;
        slider.targetGraphic = handle.GetComponent<Image>();
    }

    private static GameObject CreateUiObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        return obj;
    }

    private static TMP_Text CreateText(Transform parent, string name, string value, float size, TextAlignmentOptions alignment)
    {
        GameObject obj = CreateUiObject(name, parent);
        TMP_Text text = obj.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = size;
        text.alignment = alignment;
        text.color = Color.white;
        return text;
    }

    private static GameObject CreateButton(
        Transform parent,
        string name,
        string label,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPosition,
        Vector2 size,
        UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = CreateUiObject(name, parent);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.24f, 0.28f, 0.34f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        TMP_Text text = CreateText(buttonObject.transform, "Label", label, 20f, TextAlignmentOptions.Center);
        StretchFull(text.rectTransform);
        return buttonObject;
    }

    private static void StretchFull(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
