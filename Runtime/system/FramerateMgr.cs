using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateMgr : MonoBehaviour
{
    [Header("system")]
    public int application_targetFramerate = -1;
    
    private void Awake()
    {
        //https://docs.unity3d.com/ScriptReference/Application-targetFrameRate.html
        if (application_targetFramerate > 0)
        {
            Debug.LogWarning(GetType() + "overriding target <b>framerate to " + application_targetFramerate + "</b>");
            Application.targetFrameRate = application_targetFramerate;
            
            Debug.LogWarning(GetType() + "removing vsync");
            QualitySettings.vSyncCount = 0;
        }
    }
}
