using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultUIController : MonoBehaviour
{
    public static ResultUIController Instance { get; private set; }

    private GameObject resultRoot;
    private TMP_Text titleText;
    private TMP_Text bodyText;
    private Action continueCallback;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildResultUi();
        HideImmediate();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Show(DungeonRunResult result, Action onContinue)
    {
        if (resultRoot == null)
        {
            onContinue?.Invoke();
            return;
        }

        continueCallback = onContinue;
        titleText.text = result.Title;
        bodyText.text =
            $"Kills: {result.Kills}\n" +
            $"Gold Earned: {result.GoldEarned}\n" +
            $"Damage Dealt: {result.DamageDealt:F0}\n" +
            $"Damage Taken: {result.DamageTaken:F0}";

        resultRoot.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
    }

    public void HideImmediate()
    {
        if (resultRoot != null)
            resultRoot.SetActive(false);

        continueCallback = null;
    }

    private void Continue()
    {
        HideImmediate();
        Time.timeScale = 1f;
        Cursor.visible = false;

        Action callback = continueCallback;
        continueCallback = null;
        callback?.Invoke();
    }

    private void BuildResultUi()
    {
        resultRoot = new GameObject("ResultMenuRoot");
        resultRoot.transform.SetParent(transform, false);

        Canvas canvas = resultRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 250;

        CanvasScaler scaler = resultRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        resultRoot.AddComponent<GraphicRaycaster>();

        GameObject dim = CreateUiObject("Dim", resultRoot.transform);
        StretchFull(dim);
        dim.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.72f);

        GameObject panel = CreateUiObject("Panel", resultRoot.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(520f, 420f);
        panel.AddComponent<Image>().color = new Color(0.1f, 0.12f, 0.16f, 0.96f);

        GameObject titleObject = CreateUiObject("Title", panel.transform);
        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -24f);
        titleRect.sizeDelta = new Vector2(460f, 56f);
        titleText = titleObject.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 34f;
        titleText.alignment = TextAlignmentOptions.Center;

        GameObject bodyObject = CreateUiObject("Body", panel.transform);
        RectTransform bodyRect = bodyObject.GetComponent<RectTransform>();
        bodyRect.anchorMin = new Vector2(0.5f, 0.5f);
        bodyRect.anchorMax = new Vector2(0.5f, 0.5f);
        bodyRect.pivot = new Vector2(0.5f, 0.5f);
        bodyRect.anchoredPosition = new Vector2(0f, 20f);
        bodyRect.sizeDelta = new Vector2(460f, 220f);
        bodyText = bodyObject.AddComponent<TextMeshProUGUI>();
        bodyText.fontSize = 26f;
        bodyText.alignment = TextAlignmentOptions.TopLeft;

        CreateButton(panel.transform, "ContinueButton", "Continue", -150f, Continue);
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

    private static void CreateButton(Transform parent, string name, string label, float y, UnityEngine.Events.UnityAction action)
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
    }
}

public struct DungeonRunResult
{
    public string Title;
    public int Kills;
    public int GoldEarned;
    public float DamageDealt;
    public float DamageTaken;
}
