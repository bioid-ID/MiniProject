using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class HubUiLayoutHelper
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

        TMP_Text statusText = FindChildByName<TMP_Text>(canvas.transform, "StatusText");
        if (statusText != null)
        {
            RectTransform rect = statusText.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(24f, -24f);
            rect.sizeDelta = new Vector2(700f, 48f);
            statusText.alignment = TextAlignmentOptions.TopLeft;
        }

        Button enterButton = FindChildByName<Button>(canvas.transform, "Button");
        if (enterButton != null)
        {
            RectTransform rect = enterButton.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.anchoredPosition = new Vector2(-24f, 24f);
            rect.sizeDelta = new Vector2(220f, 52f);
        }
    }

    private static T FindChildByName<T>(Transform root, string objectName) where T : Component
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
