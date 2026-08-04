using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class DungeonUiLayoutHelper
{
    public static void Apply(Canvas canvas)
    {
        if (canvas == null)
            return;

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        if (canvas.transform.localScale == Vector3.zero)
            canvas.transform.localScale = Vector3.one;

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        LayoutTopLeft(FindChild<TMP_Text>(canvas.transform, "GoldText"), 24f, -24f, 260f, 32f);
        LayoutTopLeft(FindChild<TMP_Text>(canvas.transform, "LevelText"), 24f, -60f, 420f, 32f);
        LayoutTopLeft(FindChild<TMP_Text>(canvas.transform, "ExpText"), 24f, -96f, 420f, 28f);
        LayoutTopLeft(FindChild<TMP_Text>(canvas.transform, "KillText"), 24f, -128f, 260f, 28f);
        LayoutTopLeft(FindChild<TMP_Text>(canvas.transform, "InventoryText"), 24f, -160f, 420f, 28f);

        LayoutBottomCenter(FindChild<Slider>(canvas.transform, "HPBar"), 0f, 36f, 320f, 24f);
        LayoutScreenCenter(FindChild<TMP_Text>(canvas.transform, "DamageFeedbackText"), 0f, 72f, 240f, 48f);
    }

    private static void LayoutTopLeft(TMP_Text text, float x, float y, float width, float height)
    {
        if (text == null)
            return;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(x, y);
        rect.sizeDelta = new Vector2(width, height);
        text.alignment = TextAlignmentOptions.TopLeft;
    }

    private static void LayoutBottomCenter(Slider slider, float x, float y, float width, float height)
    {
        if (slider == null)
            return;

        RectTransform rect = slider.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(x, y);
        rect.sizeDelta = new Vector2(width, height);
    }

    private static void LayoutScreenCenter(TMP_Text text, float x, float y, float width, float height)
    {
        if (text == null)
            return;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(x, y);
        rect.sizeDelta = new Vector2(width, height);
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 34f;
    }

    private static T FindChild<T>(Transform root, string objectName) where T : Component
    {
        T[] components = root.GetComponentsInChildren<T>(true);
        foreach (T component in components)
        {
            if (component.gameObject.name == objectName)
                return component;
        }

        return null;
    }
}
