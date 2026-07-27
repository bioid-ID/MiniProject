using UnityEngine;

public static class Logger
{
#if UNITY_EDITOR
    public static void Log(object message)
    {
        Debug.Log(message);
    }

    public static void Warning(object message)
    {
        Debug.LogWarning(message);
    }

    public static void Error(object message)
    {
        Debug.LogError(message);
    }
#else
    public static void Log(object message) { }

    public static void Warning(object message) { }

    public static void Error(object message)
    {
        Debug.LogError(message);
    }
#endif
}