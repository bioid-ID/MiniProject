using System;
using System.Collections;
using UnityEngine;

public class SimpleTweenRunner : MonoBehaviour
{
    private static SimpleTweenRunner instance;

    public static SimpleTweenRunner Instance
    {
        get
        {
            if (instance != null)
                return instance;

            GameObject runnerObject = new GameObject(nameof(SimpleTweenRunner));
            instance = runnerObject.AddComponent<SimpleTweenRunner>();
            DontDestroyOnLoad(runnerObject);
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public void Play(IEnumerator routine)
    {
        if (routine == null)
            return;

        StartCoroutine(routine);
    }
}

public static class UITween
{
    public static void Fade(CanvasGroup group, float from, float to, float duration, Action onComplete = null, bool useUnscaledTime = false)
    {
        if (group == null)
        {
            onComplete?.Invoke();
            return;
        }

        SimpleTweenRunner.Instance.Play(FadeRoutine(group, from, to, duration, onComplete, useUnscaledTime));
    }

    public static void Scale(RectTransform target, Vector3 from, Vector3 to, float duration, Action onComplete = null, bool useUnscaledTime = false)
    {
        if (target == null)
        {
            onComplete?.Invoke();
            return;
        }

        SimpleTweenRunner.Instance.Play(ScaleRoutine(target, from, to, duration, onComplete, useUnscaledTime));
    }

    public static void FadeAndScale(
        CanvasGroup group,
        RectTransform target,
        bool show,
        float duration,
        Action onComplete = null,
        bool useUnscaledTime = false)
    {
        if (group == null || target == null)
        {
            onComplete?.Invoke();
            return;
        }

        float alphaFrom = show ? 0f : group.alpha;
        float alphaTo = show ? 1f : 0f;
        Vector3 scaleFrom = show ? Vector3.one * 0.92f : Vector3.one;
        Vector3 scaleTo = show ? Vector3.one : Vector3.one * 0.96f;

        group.alpha = alphaFrom;
        target.localScale = scaleFrom;

        SimpleTweenRunner.Instance.Play(CombinedRoutine(group, target, alphaFrom, alphaTo, scaleFrom, scaleTo, duration, onComplete, useUnscaledTime));
    }

    private static IEnumerator FadeRoutine(CanvasGroup group, float from, float to, float duration, Action onComplete, bool useUnscaledTime)
    {
        group.alpha = from;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
            group.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        group.alpha = to;
        onComplete?.Invoke();
    }

    private static IEnumerator ScaleRoutine(RectTransform target, Vector3 from, Vector3 to, float duration, Action onComplete, bool useUnscaledTime)
    {
        target.localScale = from;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
            target.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }

        target.localScale = to;
        onComplete?.Invoke();
    }

    private static IEnumerator CombinedRoutine(
        CanvasGroup group,
        RectTransform target,
        float alphaFrom,
        float alphaTo,
        Vector3 scaleFrom,
        Vector3 scaleTo,
        float duration,
        Action onComplete,
        bool useUnscaledTime)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            group.alpha = Mathf.Lerp(alphaFrom, alphaTo, eased);
            target.localScale = Vector3.Lerp(scaleFrom, scaleTo, eased);
            yield return null;
        }

        group.alpha = alphaTo;
        target.localScale = scaleTo;
        onComplete?.Invoke();
    }
}
