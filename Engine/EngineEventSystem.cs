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
  static protected bool pauseState = false;
  
  static public void event_toggle_pause() {
    set_pause_state(!pauseState);
  }

  static public void set_pause_state(bool flag)
  {
    if (pauseState == flag)
    {
      Debug.LogWarning("trying to swap pause state to " + flag + " but it's already at that state");
      return;
    }

    Debug.Log(getStamp()+"changed paused state to : " + flag);

    pauseState = flag;

    if (onPause != null) onPause(pauseState);
  }

  static public bool isPaused() { return pauseState; }

  static public string getStamp()
  {
    return "<color=navy>EngineEventSystem</color> | ";
  }
}
