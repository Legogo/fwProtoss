using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileMgr : MonoBehaviour
{
    [Header("mobile specifics")]
    public bool mobile_never_sleep = false;

    private void Awake()
    {
        //https://docs.unity3d.com/ScriptReference/Screen-sleepTimeout.html
        Screen.sleepTimeout = (mobile_never_sleep) ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
        Debug.Log(GetType() + " sleep timeout is setup to : <b>" + Screen.sleepTimeout + "</b> (-2 = system | -1 = never sleep)");
    }

}
