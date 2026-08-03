using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI Instance { get; private set; }

    [SerializeField] private TMP_Text promptText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void SetPrompt(string message)
    {
        if (promptText == null)
            return;

        bool hasMessage = !string.IsNullOrEmpty(message);
        promptText.gameObject.SetActive(hasMessage);
        promptText.text = message;
    }

    public void ClearPrompt()
    {
        SetPrompt(string.Empty);
    }

    public void BindPromptText(TMP_Text text)
    {
        promptText = text;
    }
}
