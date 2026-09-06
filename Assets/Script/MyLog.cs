using System.Diagnostics;
using UnityEngine;

public static class MyLog
{
    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void Log(object message)
    {
        MyLog.Log(message);
    }

    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void LogWarning(object message)
    {
        MyLog.LogWarning(message);
    }

    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void LogError(object message)
    {
        MyLog.LogError(message);
    }
}
