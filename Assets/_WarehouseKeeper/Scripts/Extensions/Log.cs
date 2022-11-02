namespace WarehouseKeeper.Extension
{
public static class Log
{
    private static string Info => "[info]";
    private static string Warning => "[warning]";
    private static string Error => "[error]";
    private static string Debug => "[debug]";
    private static string Critical => "[critical]";
    private static string Analytics => "[analytics]";

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Write(string text)
    {
#if ENABLE_LOG
        UnityEngine.Debug.Log(text);
#endif
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void WriteInfo(string text)
    {
#if ENABLE_LOG
        UnityEngine.Debug.Log($"{LogType(Info, Green)} " + text);
#endif
    }
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void WriteWarning(string text)
    {
#if ENABLE_LOG
        UnityEngine.Debug.Log($"{LogType(Warning, Orange)} " + text);
#endif
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void WriteError(string text)
    {
#if ENABLE_LOG
        UnityEngine.Debug.Log($"{LogType(Error, Red)} " + text);
#endif
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void InternalError()
    {
#if ENABLE_LOG
        UnityEngine.Debug.Log($"{LogType(Critical, Blue)} " + "Internal error");
#endif
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static string LogType(string type, string color)
    {
#if UNITY_EDITOR
        return string.Format(color, type);
#else
        return string.Concat($"[{UnityEngine.Application.identifier}]", string.Format(color, type));
#endif
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