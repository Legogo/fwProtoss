using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Usually it's when the PauseScreen is called that this function is called
/// </summary>

public class EngineEventSystem {
  
  //where scripts can subscribe to receive pause event
  static public Action<bool> onPauseEvent;
  static public Action<bool> onFocusEvent;
  
  /// <summary>
  /// if pause checker is not added by hand, this function needs to be called
  /// </summary>
  static public void create()
  {
    PauseChecker pc = qh.gc<PauseChecker>();
    if(pc == null) qh.cr<PauseChecker>("[pause]");
  }
  
  static public string getStamp()
  {
    return "<color=navy>EngineEventSystem</color> | ";
  }
}
