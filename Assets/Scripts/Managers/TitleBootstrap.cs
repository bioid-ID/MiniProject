using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-950)]
public class TitleBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInitialize()
    {
        if (!GameSceneNames.IsTitleScene(SceneManager.GetActiveScene().name))
            return;

        if (FindFirstObjectByType<TitleBootstrap>() != null)
            return;

        GameObject bootstrap = new GameObject(nameof(TitleBootstrap));
        bootstrap.AddComponent<TitleBootstrap>();
    }

    private void Awake()
    {
        BuildTitleUi();
    }

    private static void BuildTitleUi()
    {
        GameObject root = new GameObject("TitleMenuRoot");
        Object.DontDestroyOnLoad(root);

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 300;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        root.AddComponent<GraphicRaycaster>();

        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        CreateBackground(root.transform);
        CreateTitleLabel(root.transform, "Portal Dungeon", 180f);
        CreateButton(root.transform, "ContinueButton", "Continue", 20f, ContinueGame);
        CreateButton(root.transform, "NewGameButton", "New Game", -60f, StartNewGame);
        CreateButton(root.transform, "QuitButton", "Quit", -140f, QuitGame);
    }

    private static void CreateBackground(Transform parent)
    {
        GameObject background = CreateUiObject("Background", parent);
        StretchFull(background);
        background.AddComponent<Image>().color = new Color(0.05f, 0.07f, 0.12f, 1f);
    }

    private static void CreateTitleLabel(Transform parent, string title, float y)
    {
        GameObject titleObject = CreateUiObject("Title", parent);
        RectTransform rect = titleObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, y);
        rect.sizeDelta = new Vector2(800f, 96f);

        TMP_Text text = titleObject.AddComponent<TextMeshProUGUI>();
        text.text = title;
        text.fontSize = 56f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
    }

    private static void CreateButton(Transform parent, string name, string label, float y, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = CreateUiObject(name, parent);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, y);
        rect.sizeDelta = new Vector2(360f, 56f);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.22f, 0.28f, 0.36f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        GameObject labelObject = CreateUiObject("Label", buttonObject.transform);
        StretchFull(labelObject);
        TMP_Text text = labelObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 28f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
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

    private static void StartNewGame()
    {
        SaveManager.Instance?.ResetAllProgress();
        LoadHub();
    }

    private static void ContinueGame()
    {
        if (SaveManager.Instance == null || !SaveManager.Instance.HasSave())
        {
            StartNewGame();
            return;
        }

        SaveManager.Instance.Load();
        LoadHub();
    }

    private static void LoadHub()
    {
        DestroyTitleUi();

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(GameSceneNames.Hub, saveBeforeLoad: false);
        else
            SceneManager.LoadScene(GameSceneNames.Hub);
    }

    private static void DestroyTitleUi()
    {
        GameObject titleRoot = GameObject.Find("TitleMenuRoot");
        if (titleRoot != null)
            Object.Destroy(titleRoot);
    }

    private static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
