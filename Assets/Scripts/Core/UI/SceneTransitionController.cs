using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneTransitionController : MonoBehaviour
{
    public static SceneTransitionController Instance { get; private set; }

    private const float DefaultFadeDuration = 0.28f;

    private GameObject overlayRoot;
    private Image fadeImage;
    private CanvasGroup toastGroup;
    private TMP_Text toastText;
    private Coroutine fadeRoutine;
    private Coroutine toastRoutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildOverlay();
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

    public void LoadScene(string sceneName, bool saveBeforeLoad, Action beforeLoad = null, Action onLoadStarted = null)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(LoadSceneRoutine(sceneName, saveBeforeLoad, beforeLoad, onLoadStarted));
    }

    public IEnumerator FadeOut(float duration = DefaultFadeDuration)
    {
        yield return FadeTo(1f, duration);
    }

    public IEnumerator FadeIn(float duration = DefaultFadeDuration)
    {
        yield return FadeTo(0f, duration);
    }

    public void ShowToast(string message, float duration = 2f)
    {
        if (toastText == null || toastGroup == null)
            return;

        if (toastRoutine != null)
            StopCoroutine(toastRoutine);

        toastRoutine = StartCoroutine(ToastRoutine(message, duration));
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeIn(DefaultFadeDuration));
        GameStateController.Instance?.NotifySceneTransitionComplete();
    }

    private IEnumerator LoadSceneRoutine(string sceneName, bool saveBeforeLoad, Action beforeLoad, Action onLoadStarted)
    {
        yield return FadeOut(DefaultFadeDuration);

        beforeLoad?.Invoke();

        if (saveBeforeLoad)
            SaveManager.Instance?.Save();

        Debug.Log($"[SceneTransition] Loading {sceneName}...");
        SceneManager.LoadScene(sceneName);
        onLoadStarted?.Invoke();
        fadeRoutine = null;
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (fadeImage == null)
            yield break;

        overlayRoot.SetActive(true);

        float startAlpha = fadeImage.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
            SetFadeAlpha(Mathf.Lerp(startAlpha, targetAlpha, t));
            yield return null;
        }

        SetFadeAlpha(targetAlpha);

        if (targetAlpha <= 0.01f)
            overlayRoot.SetActive(false);
    }

    private IEnumerator ToastRoutine(string message, float duration)
    {
        toastText.text = message;
        toastGroup.alpha = 0f;
        toastGroup.gameObject.SetActive(true);

        float fadeIn = 0.18f;
        float elapsed = 0f;

        while (elapsed < fadeIn)
        {
            elapsed += Time.unscaledDeltaTime;
            toastGroup.alpha = Mathf.Clamp01(elapsed / fadeIn);
            yield return null;
        }

        toastGroup.alpha = 1f;
        yield return new WaitForSecondsRealtime(duration);

        elapsed = 0f;
        float fadeOut = 0.22f;

        while (elapsed < fadeOut)
        {
            elapsed += Time.unscaledDeltaTime;
            toastGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeOut);
            yield return null;
        }

        toastGroup.alpha = 0f;
        toastGroup.gameObject.SetActive(false);
        toastRoutine = null;
    }

    private void SetFadeAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    private void BuildOverlay()
    {
        overlayRoot = new GameObject("SceneTransitionOverlay");
        overlayRoot.transform.SetParent(transform, false);

        Canvas canvas = overlayRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 400;

        CanvasScaler scaler = overlayRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        overlayRoot.AddComponent<GraphicRaycaster>();

        GameObject fadeObject = CreateUiObject("Fade", overlayRoot.transform);
        StretchFull(fadeObject);
        fadeImage = fadeObject.AddComponent<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        fadeImage.raycastTarget = false;

        GameObject toastObject = CreateUiObject("Toast", overlayRoot.transform);
        RectTransform toastRect = toastObject.GetComponent<RectTransform>();
        toastRect.anchorMin = new Vector2(0.5f, 1f);
        toastRect.anchorMax = new Vector2(0.5f, 1f);
        toastRect.pivot = new Vector2(0.5f, 1f);
        toastRect.anchoredPosition = new Vector2(0f, -72f);
        toastRect.sizeDelta = new Vector2(520f, 48f);

        Image toastBackground = toastObject.AddComponent<Image>();
        toastBackground.color = new Color(0.08f, 0.1f, 0.14f, 0.88f);
        toastBackground.raycastTarget = false;

        toastGroup = toastObject.AddComponent<CanvasGroup>();
        toastGroup.alpha = 0f;
        toastObject.SetActive(false);

        GameObject toastLabelObject = CreateUiObject("Label", toastObject.transform);
        StretchFull(toastLabelObject);
        toastText = toastLabelObject.AddComponent<TextMeshProUGUI>();
        toastText.fontSize = 24f;
        toastText.alignment = TextAlignmentOptions.Center;
        toastText.color = Color.white;

        overlayRoot.SetActive(false);
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
