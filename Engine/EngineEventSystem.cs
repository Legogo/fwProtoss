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
    isPaused = !isPaused;
    if (onPause != null) onPause(isPaused);
  }
}
