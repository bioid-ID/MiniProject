using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance { get; private set; }

    public bool IsOpen { get; private set; }

    private static readonly EquipmentSlot[] EquipmentSlots =
    {
        EquipmentSlot.Weapon,
        EquipmentSlot.SubWeapon,
        EquipmentSlot.Helmet,
        EquipmentSlot.Armor,
        EquipmentSlot.Pants,
        EquipmentSlot.Gloves,
        EquipmentSlot.Boots,
        EquipmentSlot.Necklace,
        EquipmentSlot.Ring1,
        EquipmentSlot.Ring2
    };

    private GameObject inventoryRoot;
    private RectTransform panelRect;
    private CanvasGroup panelCanvasGroup;
    private RectTransform bagContentRoot;
    private RectTransform equipmentContentRoot;
    private TMP_Text headerText;
    private bool isAnimating;

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

    private void Start() => BindInventoryEvents();

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

        if (!GameInput.WasPressed(GameAction.Inventory))
            return;

        Toggle();
    }

    public void Toggle()
    {
        if (IsOpen) Close();
        else Open();
    }

    public void Open()
    {
        if (inventoryRoot == null || isAnimating)
            return;

        IsOpen = true;
        inventoryRoot.SetActive(true);
        GameStateController.Instance?.SetInventoryOpen(true);
        Refresh();
        GameEvents.RaiseInventoryOpened();

        isAnimating = true;
        UITween.FadeAndScale(panelCanvasGroup, panelRect, true, 0.18f, () => isAnimating = false);
    }

    public void Close()
    {
        if (inventoryRoot == null || !IsOpen || isAnimating)
            return;

        isAnimating = true;
        UITween.FadeAndScale(panelCanvasGroup, panelRect, false, 0.15f, FinishClose);
    }

    private void FinishClose()
    {
        isAnimating = false;
        IsOpen = false;
        if (inventoryRoot != null)
            inventoryRoot.SetActive(false);
        GameStateController.Instance?.SetInventoryOpen(false);
        GameEvents.RaiseInventoryClosed();
    }

    public void ForceClose()
    {
        isAnimating = false;
        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 0f;
        if (panelRect != null)
            panelRect.localScale = Vector3.one * 0.96f;
        if (inventoryRoot != null)
            inventoryRoot.SetActive(false);
        IsOpen = false;
        GameStateController.Instance?.SetInventoryOpen(false);
    }

    public void Refresh()
    {
        RefreshEquipment();
        RefreshBag();
    }

    private void RefreshEquipment()
    {
        if (equipmentContentRoot == null)
            return;

        ClearChildren(equipmentContentRoot);

        PlayerStat stat = PlayerStat.Instance;
        foreach (EquipmentSlot slot in EquipmentSlots)
        {
            EquipmentData equipped = stat != null ? stat.GetEquipped(slot) : null;
            string label = equipped != null ? equipped.itemName : "(Empty)";
            CreateEquipmentRow(slot, SlotLabel(slot), label, equipped != null);
        }
    }

    private void RefreshBag()
    {
        if (bagContentRoot == null)
            return;

        ClearChildren(bagContentRoot);

        Inventory inventory = Inventory.Instance;
        if (inventory == null)
        {
            CreateInfoRow(bagContentRoot, "Inventory not ready.", string.Empty);
            if (headerText != null)
                headerText.text = "Inventory";
            return;
        }

        int rowCount = 0;
        foreach (InventorySlot slot in inventory.Slots)
        {
            if (slot.IsEmpty)
                continue;

            rowCount++;
            ItemData data = slot.item.data;
            string itemName = data != null ? data.itemName : "Unknown";
            string amount = $"x{slot.item.quantity}";

            if (data is EquipmentData equipment)
                CreateBagEquipmentRow(equipment, itemName, amount);
            else
                CreateInfoRow(bagContentRoot, itemName, amount);
        }

        if (rowCount == 0)
            CreateInfoRow(bagContentRoot, "(Bag Empty)", "Loot equipment to equip");

        if (headerText != null)
            headerText.text = $"Inventory ({inventory.GetFilledSlotCount()})";
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

        GameObject panelObject = CreateUiObject("InventoryPanel", inventoryRoot.transform);
        panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(920f, 560f);

        panelCanvasGroup = panelObject.AddComponent<CanvasGroup>();
        panelCanvasGroup.alpha = 0f;
        panelObject.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 0.96f);

        headerText = CreateLabel(panelObject.transform, "Header", "Inventory",
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, 0f), new Vector2(0f, 48f), 28f, TextAlignmentOptions.MidlineLeft);
        headerText.margin = new Vector4(18f, 0f, 0f, 0f);

        TMP_Text hint = CreateLabel(panelObject.transform, "Hint",
            $"[{GameKeyBindings.GetDisplayName(GameKeyBindings.Inventory)}] Close  |  Equip / Unequip buttons manage gear",
            new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f),
            new Vector2(0f, 8f), new Vector2(0f, 28f), 18f, TextAlignmentOptions.Center);
        hint.color = new Color(0.75f, 0.75f, 0.75f, 1f);

        equipmentContentRoot = CreateScrollColumn(panelObject.transform, "EquipmentPanel",
            new Vector2(0f, 0f), new Vector2(0.42f, 1f),
            new Vector2(16f, 40f), new Vector2(-8f, -56f), "Equipment");

        bagContentRoot = CreateScrollColumn(panelObject.transform, "BagPanel",
            new Vector2(0.42f, 0f), new Vector2(1f, 1f),
            new Vector2(8f, 40f), new Vector2(-16f, -56f), "Bag");
    }

    private RectTransform CreateScrollColumn(
        Transform parent,
        string name,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offsetMin,
        Vector2 offsetMax,
        string title)
    {
        GameObject column = CreateUiObject(name, parent);
        RectTransform columnRect = column.GetComponent<RectTransform>();
        columnRect.anchorMin = anchorMin;
        columnRect.anchorMax = anchorMax;
        columnRect.offsetMin = offsetMin;
        columnRect.offsetMax = offsetMax;
        column.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.28f);

        TMP_Text titleText = CreateLabel(column.transform, "Title", title,
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, 0f), new Vector2(0f, 34f), 20f, TextAlignmentOptions.MidlineLeft);
        titleText.margin = new Vector4(12f, 0f, 0f, 0f);
        titleText.color = new Color(0.75f, 0.85f, 1f, 1f);

        GameObject scrollObject = CreateUiObject("ScrollView", column.transform);
        RectTransform scrollRectTransform = scrollObject.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = Vector2.zero;
        scrollRectTransform.anchorMax = Vector2.one;
        scrollRectTransform.offsetMin = new Vector2(8f, 8f);
        scrollRectTransform.offsetMax = new Vector2(-8f, -36f);

        ScrollRect scroll = scrollObject.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 24f;

        GameObject viewportObject = CreateUiObject("Viewport", scrollObject.transform);
        RectTransform viewportRect = viewportObject.GetComponent<RectTransform>();
        StretchFull(viewportRect);
        viewportObject.AddComponent<Image>().color = Color.white;
        Mask mask = viewportObject.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        GameObject contentObject = CreateUiObject("Content", viewportObject.transform);
        RectTransform contentRoot = contentObject.GetComponent<RectTransform>();
        contentRoot.anchorMin = new Vector2(0f, 1f);
        contentRoot.anchorMax = new Vector2(1f, 1f);
        contentRoot.pivot = new Vector2(0.5f, 1f);
        contentRoot.anchoredPosition = Vector2.zero;
        contentRoot.sizeDelta = Vector2.zero;

        VerticalLayoutGroup layout = contentObject.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 4f;
        layout.padding = new RectOffset(6, 6, 6, 6);

        ContentSizeFitter fitter = contentObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.viewport = viewportRect;
        scroll.content = contentRoot;
        return contentRoot;
    }

    private void CreateEquipmentRow(EquipmentSlot slot, string slotName, string itemName, bool canUnequip)
    {
        GameObject row = CreateRowShell(equipmentContentRoot, 42f);
        CreateRowLabel(row.transform, $"{slotName}: {itemName}", 0f, canUnequip ? 0.68f : 1f);

        if (!canUnequip)
            return;

        CreateRowButton(row.transform, "Unequip", 0.68f, () =>
        {
            GameFeel.UiClick();
            if (PlayerStat.Instance != null && PlayerStat.Instance.TryUnequipToInventory(slot))
                Refresh();
        });
    }

    private void CreateBagEquipmentRow(EquipmentData equipment, string itemName, string amount)
    {
        GameObject row = CreateRowShell(bagContentRoot, 42f);
        CreateRowLabel(row.transform, $"{itemName}  {amount}", 0f, 0.68f);
        CreateRowButton(row.transform, "Equip", 0.68f, () =>
        {
            GameFeel.UiClick();
            if (PlayerStat.Instance != null && PlayerStat.Instance.TryEquipFromInventory(equipment))
                Refresh();
        });
    }

    private void CreateInfoRow(RectTransform parent, string label, string amountText)
    {
        GameObject row = CreateRowShell(parent, 36f);
        CreateRowLabel(row.transform, label, 0f, string.IsNullOrEmpty(amountText) ? 1f : 0.7f);
        if (!string.IsNullOrEmpty(amountText))
        {
            TMP_Text amount = CreateRowLabel(row.transform, amountText, 0.7f, 1f);
            amount.alignment = TextAlignmentOptions.MidlineRight;
            amount.color = new Color(0.9f, 0.85f, 0.5f, 1f);
        }
    }

    private static GameObject CreateRowShell(Transform parent, float height)
    {
        GameObject rowObject = CreateUiObject("Row", parent);
        LayoutElement layoutElement = rowObject.AddComponent<LayoutElement>();
        layoutElement.minHeight = height;
        layoutElement.preferredHeight = height;
        rowObject.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.06f);
        return rowObject;
    }

    private static TMP_Text CreateRowLabel(Transform parent, string text, float anchorMinX, float anchorMaxX)
    {
        GameObject labelObject = CreateUiObject("Label", parent);
        RectTransform rect = labelObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(anchorMinX, 0f);
        rect.anchorMax = new Vector2(anchorMaxX, 1f);
        rect.offsetMin = new Vector2(10f, 0f);
        rect.offsetMax = new Vector2(-6f, 0f);

        TMP_Text label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = 18f;
        label.alignment = TextAlignmentOptions.MidlineLeft;
        return label;
    }

    private static void CreateRowButton(Transform parent, string label, float anchorMinX, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = CreateUiObject(label + "Button", parent);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(anchorMinX, 0.15f);
        rect.anchorMax = new Vector2(1f, 0.85f);
        rect.offsetMin = new Vector2(4f, 0f);
        rect.offsetMax = new Vector2(-8f, 0f);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.24f, 0.32f, 0.42f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        TMP_Text text = CreateLabel(buttonObject.transform, "Text", label,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
            Vector2.zero, Vector2.zero, 16f, TextAlignmentOptions.Center);
        StretchFull(text.rectTransform);
    }

    private static TMP_Text CreateLabel(
        Transform parent,
        string name,
        string text,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPosition,
        Vector2 sizeDelta,
        float fontSize,
        TextAlignmentOptions alignment)
    {
        GameObject labelObject = CreateUiObject(name, parent);
        RectTransform rect = labelObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        TMP_Text label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = alignment;
        return label;
    }

    private static void ClearChildren(Transform root)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
            Object.Destroy(root.GetChild(i).gameObject);
    }

    private static string SlotLabel(EquipmentSlot slot)
    {
        return slot switch
        {
            EquipmentSlot.SubWeapon => "Sub",
            EquipmentSlot.Ring1 => "Ring 1",
            EquipmentSlot.Ring2 => "Ring 2",
            _ => slot.ToString()
        };
    }

    private static GameObject CreateUiObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        return obj;
    }

    private static void StretchFull(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
