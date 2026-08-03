using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance { get; private set; }

    public bool IsOpen { get; private set; }

    private GameObject inventoryRoot;
    private RectTransform contentRoot;
    private TMP_Text headerText;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildInventoryUi();
        Close();
    }

    private void Start()
    {
        BindInventoryEvents();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        BindInventoryEvents();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;

        if (Inventory.Instance != null)
            Inventory.Instance.Changed -= HandleInventoryChanged;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ForceClose();
        BindInventoryEvents();
    }

    private void BindInventoryEvents()
    {
        if (Inventory.Instance == null)
            return;

        Inventory.Instance.Changed -= HandleInventoryChanged;
        Inventory.Instance.Changed += HandleInventoryChanged;
    }

    private void HandleInventoryChanged()
    {
        if (IsOpen)
            Refresh();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (GameStateController.Instance == null || !GameStateController.Instance.IsPlaying)
            return;

        if (Keyboard.current.iKey.wasPressedThisFrame)
            Toggle();
    }

    public void Toggle()
    {
        if (IsOpen)
            Close();
        else
            Open();
    }

    public void Open()
    {
        if (inventoryRoot == null)
            return;

        IsOpen = true;
        inventoryRoot.SetActive(true);
        GameStateController.Instance?.SetInventoryOpen(true);
        Refresh();
    }

    public void Close()
    {
        if (inventoryRoot == null)
            return;

        IsOpen = false;
        inventoryRoot.SetActive(false);
        GameStateController.Instance?.SetInventoryOpen(false);
    }

    public void ForceClose()
    {
        Close();
    }

    public void Refresh()
    {
        if (contentRoot == null)
            return;

        for (int i = contentRoot.childCount - 1; i >= 0; i--)
            Destroy(contentRoot.GetChild(i).gameObject);

        Inventory inventory = Inventory.Instance;
        if (inventory == null)
        {
            CreateRow("Inventory not ready.", string.Empty);
            if (headerText != null)
                headerText.text = "Inventory (0)";
            return;
        }

        int rowCount = 0;

        foreach (InventorySlot slot in inventory.Slots)
        {
            if (slot.IsEmpty)
                continue;

            rowCount++;
            string itemName = slot.item.data != null ? slot.item.data.itemName : "Unknown";
            CreateRow(itemName, $"x{slot.item.quantity}");
        }

        if (rowCount == 0)
            CreateRow("(Empty)", "Press I to close");

        if (headerText != null)
            headerText.text = $"Inventory ({inventory.GetFilledSlotCount()} slots)";
    }

    private void BuildInventoryUi()
    {
        inventoryRoot = new GameObject("InventoryMenuRoot");
        inventoryRoot.transform.SetParent(transform, false);

        Canvas canvas = inventoryRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 150;

        CanvasScaler scaler = inventoryRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        inventoryRoot.AddComponent<GraphicRaycaster>();

        GameObject panelObject = new GameObject("InventoryPanel");
        panelObject.transform.SetParent(inventoryRoot.transform, false);

        RectTransform panelRect = panelObject.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(640f, 420f);

        Image panelBackground = panelObject.AddComponent<Image>();
        panelBackground.color = new Color(0.08f, 0.08f, 0.12f, 0.94f);

        GameObject headerObject = new GameObject("Header");
        headerObject.transform.SetParent(panelObject.transform, false);

        RectTransform headerRect = headerObject.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0f, 1f);
        headerRect.anchorMax = new Vector2(1f, 1f);
        headerRect.pivot = new Vector2(0.5f, 1f);
        headerRect.anchoredPosition = Vector2.zero;
        headerRect.sizeDelta = new Vector2(-24f, 48f);

        headerText = headerObject.AddComponent<TextMeshProUGUI>();
        headerText.text = "Inventory";
        headerText.fontSize = 28f;
        headerText.alignment = TextAlignmentOptions.MidlineLeft;
        headerText.margin = new Vector4(16f, 0f, 0f, 0f);

        GameObject hintObject = new GameObject("Hint");
        hintObject.transform.SetParent(panelObject.transform, false);

        RectTransform hintRect = hintObject.AddComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(0f, 0f);
        hintRect.anchorMax = new Vector2(1f, 0f);
        hintRect.pivot = new Vector2(0.5f, 0f);
        hintRect.anchoredPosition = new Vector2(0f, 8f);
        hintRect.sizeDelta = new Vector2(-24f, 28f);

        TMP_Text hintText = hintObject.AddComponent<TextMeshProUGUI>();
        hintText.text = "[I] Close  |  [U] Use Health Potion (dungeon)";
        hintText.fontSize = 20f;
        hintText.alignment = TextAlignmentOptions.Center;
        hintText.color = new Color(0.75f, 0.75f, 0.75f, 1f);

        GameObject scrollObject = new GameObject("ScrollView");
        scrollObject.transform.SetParent(panelObject.transform, false);

        RectTransform scrollRect = scrollObject.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0f, 0f);
        scrollRect.anchorMax = new Vector2(1f, 1f);
        scrollRect.offsetMin = new Vector2(16f, 40f);
        scrollRect.offsetMax = new Vector2(-16f, -56f);

        Image scrollBackground = scrollObject.AddComponent<Image>();
        scrollBackground.color = new Color(0f, 0f, 0f, 0.35f);

        ScrollRect scroll = scrollObject.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 24f;

        GameObject viewportObject = new GameObject("Viewport");
        viewportObject.transform.SetParent(scrollObject.transform, false);

        RectTransform viewportRect = viewportObject.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;

        Image viewportImage = viewportObject.AddComponent<Image>();
        viewportImage.color = Color.white;

        Mask viewportMask = viewportObject.AddComponent<Mask>();
        viewportMask.showMaskGraphic = false;

        GameObject contentObject = new GameObject("Content");
        contentObject.transform.SetParent(viewportObject.transform, false);

        contentRoot = contentObject.AddComponent<RectTransform>();
        contentRoot.anchorMin = new Vector2(0f, 1f);
        contentRoot.anchorMax = new Vector2(1f, 1f);
        contentRoot.pivot = new Vector2(0.5f, 1f);
        contentRoot.anchoredPosition = Vector2.zero;
        contentRoot.sizeDelta = new Vector2(0f, 0f);

        VerticalLayoutGroup layout = contentObject.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 4f;
        layout.padding = new RectOffset(8, 8, 8, 8);

        ContentSizeFitter fitter = contentObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.viewport = viewportRect;
        scroll.content = contentRoot;
    }

    private void CreateRow(string label, string amountText)
    {
        GameObject rowObject = new GameObject("InventoryRow");
        rowObject.transform.SetParent(contentRoot, false);

        RectTransform rowRect = rowObject.AddComponent<RectTransform>();
        rowRect.sizeDelta = new Vector2(0f, 36f);

        LayoutElement layoutElement = rowObject.AddComponent<LayoutElement>();
        layoutElement.minHeight = 36f;
        layoutElement.preferredHeight = 36f;

        Image rowBackground = rowObject.AddComponent<Image>();
        rowBackground.color = new Color(1f, 1f, 1f, 0.06f);

        GameObject nameObject = new GameObject("Name");
        nameObject.transform.SetParent(rowObject.transform, false);

        RectTransform nameRect = nameObject.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0f, 0f);
        nameRect.anchorMax = new Vector2(0.72f, 1f);
        nameRect.offsetMin = new Vector2(12f, 0f);
        nameRect.offsetMax = Vector2.zero;

        TMP_Text nameText = nameObject.AddComponent<TextMeshProUGUI>();
        nameText.text = label;
        nameText.fontSize = 22f;
        nameText.alignment = TextAlignmentOptions.MidlineLeft;

        GameObject amountObject = new GameObject("Amount");
        amountObject.transform.SetParent(rowObject.transform, false);

        RectTransform amountRect = amountObject.AddComponent<RectTransform>();
        amountRect.anchorMin = new Vector2(0.72f, 0f);
        amountRect.anchorMax = new Vector2(1f, 1f);
        amountRect.offsetMin = Vector2.zero;
        amountRect.offsetMax = new Vector2(-12f, 0f);

        TMP_Text amountLabel = amountObject.AddComponent<TextMeshProUGUI>();
        amountLabel.text = amountText;
        amountLabel.fontSize = 22f;
        amountLabel.alignment = TextAlignmentOptions.MidlineRight;
        amountLabel.color = new Color(0.9f, 0.85f, 0.5f, 1f);
    }
}
