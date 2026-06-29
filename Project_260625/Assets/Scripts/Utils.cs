using UnityEngine;

public static class Utils
{
    public static void Log(string msg)
    {
#if UNITY_EDITOR || DEBUG
        Debug.Log(msg);
#endif
    }
}
