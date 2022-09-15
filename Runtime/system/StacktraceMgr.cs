using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StacktraceMgr : MonoBehaviour
{
    [Header("logs")]
    public bool mobile_logs_preset = false; // if true none,none,scriptonly on mobile

    public StackTraceLogType normal = StackTraceLogType.ScriptOnly;
    public StackTraceLogType warning = StackTraceLogType.ScriptOnly;
    public StackTraceLogType error = StackTraceLogType.ScriptOnly;

    static public void setupTraceLog()
    {
        StacktraceMgr mgr = GameObject.FindObjectOfType<StacktraceMgr>();
        Debug.Assert(mgr != null);

        Application.SetStackTraceLogType(LogType.Log, mgr.normal);
        Application.SetStackTraceLogType(LogType.Warning, mgr.warning);
        Application.SetStackTraceLogType(LogType.Error, mgr.error);

        if (!Application.isEditor)
        {
            //preset
            //do be too wordy on smartphones
            if (mgr.mobile_logs_preset && Application.isMobilePlatform)
            {
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
                Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
                Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
            }
        }

        Debug.Log("trace logs setup");
        Debug.Log("  L log  " + Application.GetStackTraceLogType(LogType.Log));
        Debug.Log("  L warning  " + Application.GetStackTraceLogType(LogType.Warning));
        Debug.Log("  L error  " + Application.GetStackTraceLogType(LogType.Error));

        //GlobalSettingsVolume.setupVolumes();
    }

}
