using System.Diagnostics;
using UnityEngine;

public static class MyLog
{
    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }
}
