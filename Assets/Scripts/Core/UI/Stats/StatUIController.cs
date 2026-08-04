using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatUIController : MonoBehaviour
{
    public static StatUIController Instance { get; private set; }

    public bool IsOpen { get; private set; }

    private GameObject statRoot;
    private RectTransform panelRect;
    private TMP_Text bodyText;
    private float refreshTimer;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildStatUi();
        CloseImmediate();
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
        CloseImmediate();
    }

    private void Update()
    {
        if (GameStateController.Instance == null || !GameStateController.Instance.IsPlaying)
            return;

        if (GameInput.WasPressed(GameAction.Stats))
            Toggle();

        if (!IsOpen)
            return;

        refreshTimer -= Time.deltaTime;
        if (refreshTimer <= 0f)
        {
            refreshTimer = 0.2f;
            Refresh();
        }
    }

    public void Toggle()
    {
        if (IsOpen)
            CloseImmediate();
        else
            Open();
    }

    public void Open()
    {
        if (statRoot == null)
            return;

        IsOpen = true;
        statRoot.SetActive(true);
        GameStateController.Instance?.SetStatOpen(true);
        refreshTimer = 0f;
        Refresh();
    }

    public void CloseImmediate()
    {
        if (statRoot != null)
            statRoot.SetActive(false);

        if (IsOpen)
            GameStateController.Instance?.SetStatOpen(false);

        IsOpen = false;
    }

    private void Refresh()
    {
        if (bodyText == null)
            return;

        PlayerStat stat = PlayerStat.Instance;
        if (stat == null)
        {
            bodyText.text = "No character stats available.";
            return;
        }

        float currentHp = stat.CurrentHp;
        PlayerHealth health = stat.GetComponent<PlayerHealth>();
        if (health != null)
            currentHp = health.CurrentHealth;

        string weaponName = stat.weaponSlot != null ? stat.weaponSlot.itemName : "None";

        bodyText.text =
            $"<size=22><b>CHARACTER</b></size>\n\n" +
            $"<color=#9fd4ff>Lv.{stat.CurrentLevel}</color>  " +
            $"<color=#ffd76a>Gold {stat.Gold}</color>\n" +
            $"EXP {stat.CurrentExp:F0} / {stat.MaxExp:F0}\n\n" +
            $"<b>── Primary ──</b>\n" +
            $"STR   {stat.TotalStr,4}      DEX   {stat.TotalDex,4}\n" +
            $"INT   {stat.TotalInt,4}      LUK   {stat.TotalLuck,4}\n\n" +
            $"<b>── Combat ──</b>\n" +
            $"HP    {currentHp:F0} / {stat.MaxHp:F0}   (+{stat.HpRegen:F1}/s)\n" +
            $"MP    {stat.CurrentMp:F0} / {stat.MaxMp:F0}   (+{stat.MpRegen:F1}/s)\n" +
            $"ATK   {stat.AttackDamage:F1}    DEF   {stat.Defense:F1}\n" +
            $"ASPD  {stat.AttackSpeed:F2}    MSPD  {stat.MoveSpeed:F2}\n" +
            $"CRIT  {stat.CriticalChance:F0}%    EVA   {stat.DodgeChance:F0}%\n" +
            $"LSSteal {stat.LifeSteal:P0}  ManaSteal {stat.ManaSteal:P0}\n" +
            $"Range {stat.AttackRange:F1}    Type  {stat.CurrentAttackType}\n\n" +
            $"<b>── Growth ──</b>\n" +
            $"Stat Pts {stat.StatPoints}    Passive Pts {stat.PassivePoints}\n" +
            $"Weapon   {weaponName}";
    }

    private void BuildStatUi()
    {
        statRoot = new GameObject("StatMenuRoot");
        statRoot.transform.SetParent(transform, false);

        Canvas canvas = statRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 120;

        CanvasScaler scaler = statRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        statRoot.AddComponent<GraphicRaycaster>();

        GameObject panel = CreateUiObject("Panel", statRoot.transform);
        panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 0.5f);
        panelRect.anchorMax = new Vector2(0f, 0.5f);
        panelRect.pivot = new Vector2(0f, 0.5f);
        panelRect.anchoredPosition = new Vector2(16f, 0f);
        panelRect.sizeDelta = new Vector2(292f, 460f);

        Image panelBackground = panel.AddComponent<Image>();
        panelBackground.color = new Color(0.07f, 0.09f, 0.13f, 0.88f);
        panelBackground.raycastTarget = true;

        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.35f, 0.45f, 0.65f, 0.95f);
        outline.effectDistance = new Vector2(2f, -2f);

        GameObject header = CreateUiObject("Header", panel.transform);
        RectTransform headerRect = header.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0f, 1f);
        headerRect.anchorMax = new Vector2(1f, 1f);
        headerRect.pivot = new Vector2(0.5f, 1f);
        headerRect.anchoredPosition = Vector2.zero;
        headerRect.sizeDelta = new Vector2(0f, 34f);
        Image headerBackground = header.AddComponent<Image>();
        headerBackground.color = new Color(0.12f, 0.2f, 0.34f, 0.98f);
        header.AddComponent<StatPanelDragHandle>().Bind(panelRect);

        GameObject headerLabelObject = CreateUiObject("HeaderLabel", header.transform);
        StretchFull(headerLabelObject);
        TMP_Text headerLabel = headerLabelObject.AddComponent<TextMeshProUGUI>();
        headerLabel.text = "CHARACTER";
        headerLabel.fontSize = 18f;
        headerLabel.fontStyle = FontStyles.Bold;
        headerLabel.alignment = TextAlignmentOptions.Center;
        headerLabel.color = new Color(0.88f, 0.93f, 1f, 1f);
        headerLabel.raycastTarget = false;

        GameObject portraitObject = CreateUiObject("Portrait", panel.transform);
        RectTransform portraitRect = portraitObject.GetComponent<RectTransform>();
        portraitRect.anchorMin = new Vector2(0f, 1f);
        portraitRect.anchorMax = new Vector2(0f, 1f);
        portraitRect.pivot = new Vector2(0f, 1f);
        portraitRect.anchoredPosition = new Vector2(14f, -42f);
        portraitRect.sizeDelta = new Vector2(72f, 72f);
        portraitObject.AddComponent<Image>().color = new Color(0.14f, 0.18f, 0.26f, 1f);

        GameObject portraitFrame = CreateUiObject("PortraitFrame", portraitObject.transform);
        StretchFull(portraitFrame);
        Outline portraitOutline = portraitFrame.AddComponent<Outline>();
        portraitOutline.effectColor = new Color(0.45f, 0.58f, 0.82f, 1f);
        portraitOutline.effectDistance = new Vector2(1f, -1f);

        GameObject summaryObject = CreateUiObject("Summary", panel.transform);
        RectTransform summaryRect = summaryObject.GetComponent<RectTransform>();
        summaryRect.anchorMin = new Vector2(0f, 1f);
        summaryRect.anchorMax = new Vector2(1f, 1f);
        summaryRect.pivot = new Vector2(0f, 1f);
        summaryRect.anchoredPosition = new Vector2(96f, -44f);
        summaryRect.sizeDelta = new Vector2(-110f, 68f);

        TMP_Text summaryText = summaryObject.AddComponent<TextMeshProUGUI>();
        summaryText.fontSize = 16f;
        summaryText.alignment = TextAlignmentOptions.TopLeft;
        summaryText.color = new Color(0.82f, 0.88f, 0.96f, 1f);
        summaryText.text = "Adventurer\nNexus Explorer";

        GameObject bodyObject = CreateUiObject("Body", panel.transform);
        RectTransform bodyRect = bodyObject.GetComponent<RectTransform>();
        bodyRect.anchorMin = new Vector2(0f, 0f);
        bodyRect.anchorMax = new Vector2(1f, 1f);
        bodyRect.offsetMin = new Vector2(12f, 28f);
        bodyRect.offsetMax = new Vector2(-12f, -126f);

        bodyText = bodyObject.AddComponent<TextMeshProUGUI>();
        bodyText.fontSize = 17f;
        bodyText.alignment = TextAlignmentOptions.TopLeft;
        bodyText.lineSpacing = 0f;
        bodyText.richText = true;

        GameObject hintObject = CreateUiObject("Hint", panel.transform);
        RectTransform hintRect = hintObject.GetComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(0f, 0f);
        hintRect.anchorMax = new Vector2(1f, 0f);
        hintRect.pivot = new Vector2(0.5f, 0f);
        hintRect.anchoredPosition = new Vector2(0f, 8f);
        hintRect.sizeDelta = new Vector2(-16f, 22f);

        TMP_Text hintText = hintObject.AddComponent<TextMeshProUGUI>();
        hintText.text = $"[{GameKeyBindings.GetDisplayName(GameKeyBindings.Stats)}] Toggle  |  Drag header to move";
        hintText.fontSize = 14f;
        hintText.alignment = TextAlignmentOptions.Center;
        hintText.color = new Color(0.65f, 0.72f, 0.82f, 1f);
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
}

public class StatPanelDragHandle : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private RectTransform panelRect;
    private Vector2 dragOffset;

    public void Bind(RectTransform target)
    {
        panelRect = target;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (panelRect == null)
            return;

        RectTransform parentRect = panelRect.parent as RectTransform;
        if (parentRect == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMouse);

        dragOffset = panelRect.anchoredPosition - localMouse;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (panelRect == null)
            return;

        RectTransform parentRect = panelRect.parent as RectTransform;
        if (parentRect == null)
            return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localMouse))
        {
            panelRect.anchoredPosition = localMouse + dragOffset;
        }
    }
}
