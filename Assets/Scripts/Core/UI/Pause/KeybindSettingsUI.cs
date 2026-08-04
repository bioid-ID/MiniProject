using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeybindSettingsUI : MonoBehaviour
{
    private GameAction? pendingAction;
    private TMP_Text pendingHintText;
    private readonly System.Collections.Generic.Dictionary<GameAction, TMP_Text> labels =
        new System.Collections.Generic.Dictionary<GameAction, TMP_Text>();

    public void BuildInto(Transform parent, float startY)
    {
        labels.Clear();
        pendingAction = null;
        pendingHintText = null;

        float y = startY;
        foreach (GameAction action in Enum.GetValues(typeof(GameAction)))
        {
            if (action == GameAction.Pause)
                continue;

            CreateKeybindRow(parent, action, ref y);
        }

        CreateActionButton(parent, "ResetKeysButton", "Reset Keys", y - 8f, ResetKeys);

        GameObject hintObject = CreateUiObject("RebindHint", parent);
        RectTransform hintRect = hintObject.GetComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(0f, 1f);
        hintRect.anchorMax = new Vector2(1f, 1f);
        hintRect.pivot = new Vector2(0f, 1f);
        hintRect.anchoredPosition = new Vector2(20f, y - 52f);
        hintRect.sizeDelta = new Vector2(-40f, 24f);

        pendingHintText = hintObject.AddComponent<TextMeshProUGUI>();
        pendingHintText.fontSize = 16f;
        pendingHintText.alignment = TextAlignmentOptions.MidlineLeft;
        pendingHintText.color = new Color(0.85f, 0.9f, 1f, 0.9f);
        pendingHintText.text = string.Empty;

        RefreshLabels();
    }

    // Legacy entry point kept for safety if old callers remain.
    public void Build(Transform parent, float titleY = -370f)
    {
        BuildInto(parent, titleY);
    }

    private void Update()
    {
        if (pendingAction == null || Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelRebind();
            return;
        }

        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            if (key == Key.None)
                continue;

            if (!Keyboard.current[key].wasPressedThisFrame)
                continue;

            GameKeyBindings.Set(pendingAction.Value, key);
            CancelRebind();
            RefreshLabels();
            return;
        }
    }

    private void CreateKeybindRow(Transform parent, GameAction action, ref float y)
    {
        GameObject row = CreateUiObject($"Keybind_{action}", parent);
        RectTransform rowRect = row.GetComponent<RectTransform>();
        rowRect.anchorMin = new Vector2(0f, 1f);
        rowRect.anchorMax = new Vector2(1f, 1f);
        rowRect.pivot = new Vector2(0f, 1f);
        rowRect.anchoredPosition = new Vector2(20f, y);
        rowRect.sizeDelta = new Vector2(-40f, 34f);

        GameObject labelObject = CreateUiObject("Label", row.transform);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 0f);
        labelRect.anchorMax = new Vector2(0.45f, 1f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TMP_Text labelText = labelObject.AddComponent<TextMeshProUGUI>();
        labelText.text = GameKeyBindings.GetLabel(action);
        labelText.fontSize = 17f;
        labelText.alignment = TextAlignmentOptions.MidlineLeft;

        GameObject buttonObject = CreateUiObject("Button", row.transform);
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.48f, 0.1f);
        buttonRect.anchorMax = new Vector2(0.82f, 0.9f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.22f, 0.26f, 0.32f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;

        GameObject keyLabelObject = CreateUiObject("KeyLabel", buttonObject.transform);
        StretchFull(keyLabelObject);
        TMP_Text keyLabel = keyLabelObject.AddComponent<TextMeshProUGUI>();
        keyLabel.fontSize = 16f;
        keyLabel.alignment = TextAlignmentOptions.Center;
        labels[action] = keyLabel;

        GameAction capturedAction = action;
        button.onClick.AddListener(() => BeginRebind(capturedAction));

        y -= 40f;
    }

    private void BeginRebind(GameAction action)
    {
        pendingAction = action;
        if (pendingHintText != null)
            pendingHintText.text = $"Press a key for {GameKeyBindings.GetLabel(action)} (ESC cancel)";
    }

    private void CancelRebind()
    {
        pendingAction = null;
        if (pendingHintText != null)
            pendingHintText.text = string.Empty;
    }

    private void ResetKeys()
    {
        GameFeel.UiClick();
        GameKeyBindings.ResetDefaults();
        RefreshLabels();
        CancelRebind();
    }

    private void RefreshLabels()
    {
        foreach (var pair in labels)
        {
            if (pair.Value != null)
                pair.Value.text = GameKeyBindings.GetDisplayName(GameInput.GetKey(pair.Key));
        }
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

    private static void CreateActionButton(Transform parent, string name, string label, float y, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = CreateUiObject(name, parent);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(20f, y);
        rect.sizeDelta = new Vector2(220f, 36f);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.24f, 0.28f, 0.34f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        GameObject labelObject = CreateUiObject("Label", buttonObject.transform);
        StretchFull(labelObject);
        TMP_Text text = labelObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 18f;
        text.alignment = TextAlignmentOptions.Center;
    }
}
