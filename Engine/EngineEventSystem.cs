using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Usually it's when the PauseScreen is called that this function is called
/// </summary>

public class EngineEventSystem {
  
  //where scripts can subscribe to receive pause event
  static public Action<bool> onPause;
  static protected bool isPaused = false;
  
  static public void event_toggle_pause() {
    set_pause_state(!isPaused);
  }

  static public void set_pause_state(bool flag)
  {
    if (isPaused == flag)
    {
      Debug.LogWarning("trying to swap pause state to " + flag + " but it's already at that state");
      return;
    }

    Debug.Log(getStamp()+"changed paused state to : " + flag);

    isPaused = flag;

    if (onPause != null) onPause(isPaused);
  }

  static public string getStamp()
  {
    return "<color=navy>EngineEventSystem</color> | ";
  }
}
