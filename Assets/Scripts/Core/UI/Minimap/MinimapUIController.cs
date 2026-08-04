using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MinimapUIController : MonoBehaviour
{
    public static MinimapUIController Instance { get; private set; }

    private const float MapHalfExtent = 14f;
    private const float PanelSize = 196f;

    private GameObject minimapRoot;
    private RectTransform blipContainer;
    private Image playerBlip;
    private readonly List<Image> enemyBlips = new List<Image>();
    private readonly List<Image> portalBlips = new List<Image>();
    private TMP_Text labelText;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildMinimapUi();
        SetVisible(false);
    }

    private void OnEnable()
    {
        if (GameStateController.Instance != null)
            GameStateController.Instance.ContextChanged += HandleContextChanged;

        SceneManager.sceneLoaded += HandleSceneLoaded;
        RefreshVisibility();
    }

    private void OnDisable()
    {
        if (GameStateController.Instance != null)
            GameStateController.Instance.ContextChanged -= HandleContextChanged;

        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void LateUpdate()
    {
        if (minimapRoot == null || !minimapRoot.activeSelf)
            return;

        UpdateBlips();
    }

    private void HandleContextChanged(GameContext context)
    {
        RefreshVisibility();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshVisibility();
    }

    private void RefreshVisibility()
    {
        SetVisible(GameStateController.Instance != null && GameStateController.Instance.Context == GameContext.Dungeon);
    }

    private void SetVisible(bool visible)
    {
        if (minimapRoot != null)
            minimapRoot.SetActive(visible);
    }

    private void UpdateBlips()
    {
        Transform playerTransform = PlayerSpawnUtility.FindExistingPlayer()?.transform;
        if (playerTransform == null)
            return;

        Vector2 playerPos = playerTransform.position;
        playerBlip.rectTransform.anchoredPosition = Vector2.zero;

        UpdateEnemyBlips(playerPos);
        UpdatePortalBlips(playerPos);
    }

    private void UpdateEnemyBlips(Vector2 playerPos)
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        EnsureBlipCount(enemyBlips, enemies.Length, new Color(0.95f, 0.28f, 0.28f, 1f), 8f);

        int activeCount = 0;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy)
                continue;

            if (activeCount >= enemyBlips.Count)
                break;

            Image blip = enemyBlips[activeCount];
            blip.gameObject.SetActive(true);
            blip.rectTransform.anchoredPosition = WorldToMinimap(enemy.transform.position - (Vector3)playerPos);
            activeCount++;
        }

        for (int i = activeCount; i < enemyBlips.Count; i++)
            enemyBlips[i].gameObject.SetActive(false);
    }

    private void UpdatePortalBlips(Vector2 playerPos)
    {
        PortalTrigger[] portals = FindObjectsByType<PortalTrigger>(FindObjectsSortMode.None);
        EnsureBlipCount(portalBlips, portals.Length, new Color(0.35f, 0.95f, 0.55f, 1f), 10f);

        int activeCount = 0;

        foreach (PortalTrigger portal in portals)
        {
            if (portal == null || !portal.gameObject.activeInHierarchy)
                continue;

            if (activeCount >= portalBlips.Count)
                break;

            Image blip = portalBlips[activeCount];
            blip.gameObject.SetActive(true);
            blip.rectTransform.anchoredPosition = WorldToMinimap(portal.transform.position - (Vector3)playerPos);
            activeCount++;
        }

        for (int i = activeCount; i < portalBlips.Count; i++)
            portalBlips[i].gameObject.SetActive(false);
    }

    private static Vector2 WorldToMinimap(Vector3 worldOffset)
    {
        float scale = PanelSize / (MapHalfExtent * 2f);
        Vector2 mapped = new Vector2(worldOffset.x, worldOffset.y) * scale;
        float limit = PanelSize * 0.5f - 6f;
        return Vector2.ClampMagnitude(mapped, limit);
    }

    private void EnsureBlipCount(List<Image> blips, int requiredCount, Color color, float size)
    {
        while (blips.Count < requiredCount)
            blips.Add(CreateBlip(color, size));
    }

    private Image CreateBlip(Color color, float size)
    {
        GameObject blipObject = new GameObject("Blip", typeof(RectTransform));
        blipObject.transform.SetParent(blipContainer, false);

        RectTransform rect = blipObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(size, size);

        Image image = blipObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private void BuildMinimapUi()
    {
        minimapRoot = new GameObject("MinimapRoot");
        minimapRoot.transform.SetParent(transform, false);

        Canvas canvas = minimapRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 90;

        CanvasScaler scaler = minimapRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        GameObject panelObject = new GameObject("Panel", typeof(RectTransform));
        panelObject.transform.SetParent(minimapRoot.transform, false);

        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1f, 1f);
        panelRect.anchorMax = new Vector2(1f, 1f);
        panelRect.pivot = new Vector2(1f, 1f);
        panelRect.anchoredPosition = new Vector2(-20f, -20f);
        panelRect.sizeDelta = new Vector2(PanelSize, PanelSize);

        Image panelBackground = panelObject.AddComponent<Image>();
        panelBackground.color = new Color(0.04f, 0.06f, 0.1f, 0.78f);
        panelBackground.raycastTarget = false;

        GameObject borderObject = new GameObject("Border", typeof(RectTransform));
        borderObject.transform.SetParent(panelObject.transform, false);
        StretchFull(borderObject);
        Outline outline = borderObject.AddComponent<Outline>();
        outline.effectColor = new Color(0.45f, 0.55f, 0.7f, 0.9f);
        outline.effectDistance = new Vector2(2f, -2f);

        GameObject labelObject = new GameObject("Label", typeof(RectTransform));
        labelObject.transform.SetParent(panelObject.transform, false);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 1f);
        labelRect.anchorMax = new Vector2(1f, 1f);
        labelRect.pivot = new Vector2(0.5f, 1f);
        labelRect.anchoredPosition = new Vector2(0f, 6f);
        labelRect.sizeDelta = new Vector2(0f, 22f);

        labelText = labelObject.AddComponent<TextMeshProUGUI>();
        labelText.text = "Minimap";
        labelText.fontSize = 14f;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.color = new Color(0.85f, 0.9f, 1f, 0.95f);
        labelText.raycastTarget = false;

        GameObject blipRootObject = new GameObject("Blips", typeof(RectTransform));
        blipRootObject.transform.SetParent(panelObject.transform, false);
        blipContainer = blipRootObject.GetComponent<RectTransform>();
        StretchRect(blipContainer);

        GameObject playerObject = new GameObject("PlayerBlip", typeof(RectTransform));
        playerObject.transform.SetParent(blipContainer, false);
        RectTransform playerRect = playerObject.GetComponent<RectTransform>();
        playerRect.sizeDelta = new Vector2(12f, 12f);
        playerBlip = playerObject.AddComponent<Image>();
        playerBlip.color = new Color(0.35f, 0.75f, 1f, 1f);
        playerBlip.raycastTarget = false;
    }

    private static void StretchFull(GameObject obj)
    {
        StretchRect(obj.GetComponent<RectTransform>());
    }

    private static void StretchRect(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
