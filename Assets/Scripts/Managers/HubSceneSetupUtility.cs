using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class HubSceneSetupUtility
{
    public static void Apply()
    {
        SetupHubPlayer();
        FixHubCamera();
        FixHubCanvas();
        EnsureInteractionUI();
        RefreshHubUI();
    }

    private static void SetupHubPlayer()
    {
        GameObject playerObject = PlayerSpawnUtility.EnsurePlayer(PlayerSetupMode.Hub, Vector3.zero);
        if (playerObject == null)
            Debug.LogWarning("HubSceneSetupUtility: Failed to ensure hub player.");
    }

    private static void FixHubCamera()
    {
        GameObject cinemachineObject = GameObject.Find("mainCamera");
        if (cinemachineObject != null)
            cinemachineObject.SetActive(false);

        Camera camera = Camera.main;
        if (camera == null)
            return;

        camera.orthographic = true;
        camera.orthographicSize = 5f;

        if (Mathf.Abs(camera.transform.position.z) < 0.01f)
            camera.transform.position = new Vector3(0f, 0f, -10f);

        SimpleCameraFollow follow = camera.GetComponent<SimpleCameraFollow>();
        if (follow == null)
            follow = camera.gameObject.AddComponent<SimpleCameraFollow>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            follow.SetTarget(playerObject.transform, snapImmediate: true);
    }

    private static void FixHubCanvas()
    {
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas canvas in canvases)
            HubUiLayoutHelper.Apply(canvas);
    }

    private static void EnsureInteractionUI()
    {
        if (InteractionPromptUI.Instance != null)
            return;

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return;

        GameObject promptObject = new GameObject("InteractionPrompt");
        promptObject.transform.SetParent(canvas.transform, false);

        RectTransform rect = promptObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 48f);
        rect.sizeDelta = new Vector2(900f, 56f);

        TMP_Text promptText = promptObject.AddComponent<TextMeshProUGUI>();
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.fontSize = 26f;
        promptText.color = Color.white;

        InteractionPromptUI promptUI = promptObject.AddComponent<InteractionPromptUI>();
        promptUI.BindPromptText(promptText);
    }

    private static void RefreshHubUI()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return;

        HubUiLayoutHelper.Apply(canvas);

        LobbyUI lobbyUI = canvas.GetComponent<LobbyUI>();
        if (lobbyUI == null)
            lobbyUI = canvas.gameObject.AddComponent<LobbyUI>();

        TMP_Text statusText = null;
        foreach (TMP_Text text in canvas.GetComponentsInChildren<TMP_Text>(true))
        {
            if (text.gameObject.name == "StatusText")
            {
                statusText = text;
                break;
            }
        }

        if (statusText != null)
            lobbyUI.BindStatusText(statusText);

        foreach (Button button in canvas.GetComponentsInChildren<Button>(true))
        {
            if (button.gameObject.name == "Button")
            {
                lobbyUI.BindEnterButton(button);
                break;
            }
        }

        lobbyUI.RefreshStatus();
    }
}
