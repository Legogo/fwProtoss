using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// https://docs.unity3d.com/ScriptReference/Debug.Log.html
/// https://answers.unity.com/questions/877553/debuglog-override.html
/// </summary>

public class Logger {

  [RuntimeInitializeOnLoadMethod]
  static private void create()
  {
    //Application.logMessageReceived += log;
  }
	
  static private void log(string logString, string stackTrace, LogType type)
  {

  }

}
