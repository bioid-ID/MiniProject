using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultUIController : MonoBehaviour
{
    public static ResultUIController Instance { get; private set; }

    private enum ResultSection
    {
        Summary,
        Damage,
        Earnings,
        Recovery,
        Taken
    }

    private GameObject hubButtonRoot;
    private GameObject resultRoot;
    private RectTransform resultPanelRect;
    private CanvasGroup resultPanelCanvasGroup;
    private TMP_Text titleText;
    private TMP_Text bodyText;
    private RectTransform toggleRow;
    private Button hubToggleButton;
    private TMP_Text hubToggleLabel;

    private bool hasLastResult;
    private bool panelOpen;
    private DungeonRunResult currentResult;

    private readonly Dictionary<ResultSection, bool> sectionEnabled = new()
    {
        { ResultSection.Summary, true },
        { ResultSection.Damage, true },
        { ResultSection.Earnings, true },
        { ResultSection.Recovery, true },
        { ResultSection.Taken, false }
    };

    private readonly Dictionary<ResultSection, Button> sectionButtons = new();

    public bool HasLastResult => hasLastResult;
    public bool IsPanelOpen => panelOpen;

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
        BuildResultUi();
        HidePanel();
        SetHubButtonVisible(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HidePanel();

        if (GameSceneNames.IsHubScene(scene.name) && hasLastResult)
            SetHubButtonVisible(true);
        else
            SetHubButtonVisible(false);
    }

    public void StoreLastResult(DungeonRunResult result)
    {
        currentResult = result;
        hasLastResult = true;
    }

    public void ClearLastResult()
    {
        hasLastResult = false;
        HidePanel();
        SetHubButtonVisible(false);
    }

    public void OnReturnedToHub()
    {
        if (!hasLastResult)
        {
            SetHubButtonVisible(false);
            return;
        }

        SetHubButtonVisible(true);
        // Keep closed by default — player opens with the hub button.
        HidePanel();
    }

    // Legacy API kept so old callers don't break.
    public void Show(DungeonRunResult result, Action onContinue = null, float autoContinueSeconds = 0f)
    {
        StoreLastResult(result);
        onContinue?.Invoke();
    }

    public void HideImmediate()
    {
        HidePanel();
    }

    public void HidePanel()
    {
        panelOpen = false;
        if (resultRoot != null)
            resultRoot.SetActive(false);
        RefreshHubButtonLabel();
    }

    public void TogglePanel()
    {
        if (!hasLastResult)
            return;

        GameFeel.UiClick();

        if (panelOpen)
            HidePanel();
        else
            OpenPanel();
    }

    private void OpenPanel()
    {
        if (!hasLastResult || resultRoot == null)
            return;

        panelOpen = true;
        titleText.text = string.IsNullOrEmpty(currentResult.Title) ? "Run Report" : currentResult.Title;
        resultRoot.SetActive(true);
        UiEventSystemUtility.EnsureExists();

        RefreshBody();
        RefreshToggleVisuals();
        RefreshHubButtonLabel();

        resultPanelCanvasGroup.alpha = 0f;
        resultPanelRect.localScale = Vector3.one * 0.96f;
        UITween.FadeAndScale(resultPanelCanvasGroup, resultPanelRect, true, 0.18f, useUnscaledTime: true);
    }

    private void SetHubButtonVisible(bool visible)
    {
        if (hubButtonRoot != null)
            hubButtonRoot.SetActive(visible);

        RefreshHubButtonLabel();
    }

    private void RefreshHubButtonLabel()
    {
        if (hubToggleLabel == null)
            return;

        hubToggleLabel.text = panelOpen ? "Hide Report" : "Run Report";
    }

    private void ToggleSection(ResultSection section)
    {
        GameFeel.UiClick();
        sectionEnabled[section] = !sectionEnabled[section];
        RefreshToggleVisuals();
        RefreshBody();
    }

    private void RefreshToggleVisuals()
    {
        foreach (var pair in sectionButtons)
        {
            Image image = pair.Value.targetGraphic as Image;
            if (image == null)
                continue;

            bool on = sectionEnabled[pair.Key];
            image.color = on
                ? new Color(0.28f, 0.48f, 0.72f, 1f)
                : new Color(0.18f, 0.2f, 0.24f, 1f);
        }
    }

    private void RefreshBody()
    {
        if (bodyText == null || !hasLastResult)
            return;

        StringBuilder sb = new StringBuilder(512);

        if (sectionEnabled[ResultSection.Summary])
        {
            sb.AppendLine("<b>── Summary ──</b>");
            sb.AppendLine($"Kills            {currentResult.Kills}");
            sb.AppendLine($"Hits Landed      {currentResult.HitsLanded}  (Crit {currentResult.CritHits})");
            sb.AppendLine($"Damage Dealt     {currentResult.DamageDealt:F0}");
            sb.AppendLine($"Damage Taken     {currentResult.DamageTaken:F0}");
            sb.AppendLine();
        }

        if (sectionEnabled[ResultSection.Damage])
        {
            sb.AppendLine("<b>── Damage by Method ──</b>");
            AppendMap(sb, currentResult.DamageByMethod, FormatMethod);
            sb.AppendLine();
            sb.AppendLine("<b>── Damage by Type ──</b>");
            AppendMap(sb, currentResult.DamageByType, t => t.ToString());
            sb.AppendLine();
        }

        if (sectionEnabled[ResultSection.Earnings])
        {
            sb.AppendLine("<b>── Earnings ──</b>");
            sb.AppendLine($"Gold Earned      {currentResult.GoldEarned}");
            sb.AppendLine($"Kills            {currentResult.Kills}");
            sb.AppendLine();
        }

        if (sectionEnabled[ResultSection.Recovery])
        {
            sb.AppendLine("<b>── Recovery ──</b>");
            sb.AppendLine($"HP Healed        {currentResult.Healing:F0}");
            AppendMap(sb, currentResult.HealingBySource, s => s.ToString());
            sb.AppendLine($"MP Restored      {currentResult.ManaRestored:F0}");
            AppendMap(sb, currentResult.ManaBySource, s => s.ToString());
            sb.AppendLine();
        }

        if (sectionEnabled[ResultSection.Taken])
        {
            sb.AppendLine("<b>── Damage Taken ──</b>");
            sb.AppendLine($"Total Taken      {currentResult.DamageTaken:F0}");
            sb.AppendLine();
        }

        if (sb.Length == 0)
            sb.Append("All sections are off. Toggle buttons above to show details.");

        bodyText.text = sb.ToString().TrimEnd();
    }

    private static void AppendMap<T>(StringBuilder sb, Dictionary<T, float> map, Func<T, string> labeler)
    {
        if (map == null || map.Count == 0)
        {
            sb.AppendLine("  (none)");
            return;
        }

        foreach (var pair in map)
        {
            if (pair.Value <= 0.01f)
                continue;

            sb.AppendLine($"  {labeler(pair.Key),-14} {pair.Value:F0}");
        }
    }

    private static string FormatMethod(AttackMethod method)
    {
        return method switch
        {
            AttackMethod.Melee => "Melee",
            AttackMethod.Projectile => "Projectile",
            AttackMethod.Skill => "Skill",
            _ => "Other"
        };
    }

    private void BuildResultUi()
    {
        BuildHubToggleButton();

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
        Image dimImage = dim.AddComponent<Image>();
        dimImage.color = new Color(0f, 0f, 0f, 0.45f);
        dimImage.raycastTarget = true;
        Button dimButton = dim.AddComponent<Button>();
        dimButton.targetGraphic = dimImage;
        dimButton.onClick.AddListener(HidePanel);

        GameObject panel = CreateUiObject("Panel", resultRoot.transform);
        resultPanelRect = panel.GetComponent<RectTransform>();
        resultPanelRect.anchorMin = new Vector2(0.5f, 0.5f);
        resultPanelRect.anchorMax = new Vector2(0.5f, 0.5f);
        resultPanelRect.pivot = new Vector2(0.5f, 0.5f);
        resultPanelRect.sizeDelta = new Vector2(640f, 640f);
        panel.AddComponent<Image>().color = new Color(0.1f, 0.12f, 0.16f, 0.96f);
        resultPanelCanvasGroup = panel.AddComponent<CanvasGroup>();

        GameObject titleObject = CreateUiObject("Title", panel.transform);
        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -18f);
        titleRect.sizeDelta = new Vector2(560f, 48f);
        titleText = titleObject.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 32f;
        titleText.alignment = TextAlignmentOptions.Center;

        toggleRow = CreateUiObject("ToggleRow", panel.transform).GetComponent<RectTransform>();
        toggleRow.anchorMin = new Vector2(0.5f, 1f);
        toggleRow.anchorMax = new Vector2(0.5f, 1f);
        toggleRow.pivot = new Vector2(0.5f, 1f);
        toggleRow.anchoredPosition = new Vector2(0f, -72f);
        toggleRow.sizeDelta = new Vector2(580f, 40f);

        CreateToggle(ResultSection.Summary, "Summary", 0);
        CreateToggle(ResultSection.Damage, "Damage", 1);
        CreateToggle(ResultSection.Earnings, "Earn", 2);
        CreateToggle(ResultSection.Recovery, "Heal", 3);
        CreateToggle(ResultSection.Taken, "Taken", 4);

        GameObject scrollObject = CreateUiObject("Scroll", panel.transform);
        RectTransform scrollRectTransform = scrollObject.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        scrollRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRectTransform.pivot = new Vector2(0.5f, 0.5f);
        scrollRectTransform.anchoredPosition = new Vector2(0f, 10f);
        scrollRectTransform.sizeDelta = new Vector2(560f, 420f);
        scrollObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.25f);

        ScrollRect scroll = scrollObject.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 28f;

        GameObject viewport = CreateUiObject("Viewport", scrollObject.transform);
        RectTransform viewportRect = viewport.GetComponent<RectTransform>();
        StretchFull(viewport);
        viewport.AddComponent<Image>().color = Color.white;
        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        GameObject content = CreateUiObject("Content", viewport.transform);
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0f, 0f);

        bodyText = content.AddComponent<TextMeshProUGUI>();
        bodyText.fontSize = 22f;
        bodyText.alignment = TextAlignmentOptions.TopLeft;
        bodyText.richText = true;
        bodyText.margin = new Vector4(14f, 10f, 14f, 10f);

        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.viewport = viewportRect;
        scroll.content = contentRect;

        CreateButton(panel.transform, "CloseButton", "Close", -280f, HidePanel);
    }

    private void BuildHubToggleButton()
    {
        hubButtonRoot = new GameObject("RunReportButtonRoot");
        hubButtonRoot.transform.SetParent(transform, false);

        Canvas canvas = hubButtonRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 180;

        CanvasScaler scaler = hubButtonRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        hubButtonRoot.AddComponent<GraphicRaycaster>();

        GameObject buttonObject = CreateUiObject("RunReportButton", hubButtonRoot.transform);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(1f, 0f);
        rect.anchoredPosition = new Vector2(-24f, 24f);
        rect.sizeDelta = new Vector2(180f, 48f);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.18f, 0.28f, 0.42f, 0.95f);

        hubToggleButton = buttonObject.AddComponent<Button>();
        hubToggleButton.targetGraphic = image;
        hubToggleButton.onClick.AddListener(TogglePanel);

        GameObject labelObject = CreateUiObject("Label", buttonObject.transform);
        StretchFull(labelObject);
        hubToggleLabel = labelObject.AddComponent<TextMeshProUGUI>();
        hubToggleLabel.text = "Run Report";
        hubToggleLabel.fontSize = 20f;
        hubToggleLabel.alignment = TextAlignmentOptions.Center;
        hubToggleLabel.color = Color.white;
    }

    private void CreateToggle(ResultSection section, string label, int index)
    {
        float width = 108f;
        float gap = 8f;
        float total = 5f * width + 4f * gap;
        float startX = -total * 0.5f + width * 0.5f;

        GameObject buttonObject = CreateUiObject($"Toggle_{section}", toggleRow);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(startX + index * (width + gap), 0f);
        rect.sizeDelta = new Vector2(width, 34f);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.28f, 0.48f, 0.72f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        ResultSection captured = section;
        button.onClick.AddListener(() => ToggleSection(captured));
        sectionButtons[section] = button;

        GameObject labelObject = CreateUiObject("Label", buttonObject.transform);
        StretchFull(labelObject);
        TMP_Text text = labelObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 16f;
        text.alignment = TextAlignmentOptions.Center;
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
    public float Healing;
    public float ManaRestored;
    public int CritHits;
    public int HitsLanded;
    public Dictionary<AttackMethod, float> DamageByMethod;
    public Dictionary<DamageType, float> DamageByType;
    public Dictionary<HealSource, float> HealingBySource;
    public Dictionary<ManaSource, float> ManaBySource;
}
