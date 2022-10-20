using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this is a standard implem
/// you can use any other enum with verbose flow
/// </summary>
public enum VerbosityLevel
{
    GameKeyEvents,
    Loading,
    Localization,
    Addressables,
    Factory,
    Indus,
}

public class VerboseToggles
{
    static public bool CheckVerbosityToggle(Enum verbosityLevel)
	{
#if UNITY_EDITOR
        return UnityEditor.EditorPrefs.GetInt(verbosityLevel.ToString(), 0) == 1;
#else
        return false;
#endif
    }

    public static void Log(Enum verbosityLevel, string str, UnityEngine.Object who = null)
    {
        if (CheckVerbosityToggle(verbosityLevel) == false)
            return;

        Debug.Log($"(({verbosityLevel})) {str}", who);
    }

    public static void LogWarning(Enum verbosityLevel, string str, UnityEngine.Object who = null)
    {
        if (CheckVerbosityToggle(verbosityLevel) == false)
            return;

        Debug.LogWarning($"({verbosityLevel}) {str}", who);
    }

    public static void LogError(Enum verbosityLevel, string str, UnityEngine.Object who = null)
    {
        if (CheckVerbosityToggle(verbosityLevel) == false)
            return;

        Debug.LogError($"({verbosityLevel}) {str}", who);
    }
}
