using System.Runtime.CompilerServices;
using Game.InternalData;
using UnityEngine;

namespace Game
{
public static class Log
{
    private static string InfoType => "[info]";
    private static string WarningType => "[warning]";
    private static string ErrorType => "[error]";
    private static string ExceptionType => "[exception]";
    private static string DebugType => "[debug]";
    private static string CriticalType => "[critical]";
    private static string AnalyticsType => "[analytics]";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string text)
    {
#if ENABLE_LOG
        Debug.Log(text);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Info(string text)
    {
#if ENABLE_LOG
        Debug.Log($"{LogType(InfoType, Green)} " + text);
#endif
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Warning(string text)
    {
#if ENABLE_LOG
        Debug.Log($"{LogType(WarningType, Orange)} " + text);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Error(string text)
    {
#if ENABLE_LOG
        Debug.Log($"{LogType(ErrorType, Red)} " + text);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Exception(string text)
    {
#if ENABLE_LOG
        Debug.Log($"{LogType(ExceptionType, Pink)} " + text);
#endif
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InternalError()
    {
#if ENABLE_LOG
        Debug.Log($"{LogType(CriticalType, Blue)} " + "Internal error");
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string LogType(string type, string color)
    {
        return Application.isEditor ? 
            string.Format(color, type) : 
            string.Concat($"[{GameData.Identifier}]", string.Format(color, type));
    }

    #region Colors

    private static string NonColor => "{0}";
    private static string Green => "<color=#00FF00>{0}</color>";
    private static string Orange => "<color=#FF8000>{0}</color>";
    private static string Red => "<color=#FF5151>{0}</color>";
    private static string Blue => "<color=#0000FF>{0}</color>";
    private static string Cyan => "<color=#00FFFF>{0}</color>";
    private static string Yellow => "<color=#FFFF00>{0}</color>";
    private static string Violet => "<color=#8F00FF>{0}</color>";
    private static string Pink => "<color=#FFC0CB>{0}</color>";

    #endregion
}
}