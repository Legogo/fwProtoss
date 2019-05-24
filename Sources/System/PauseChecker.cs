using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// meant to be the funnel point to catch a pause event
/// </summary>

public class PauseChecker : MonoBehaviour {
  
  private void OnApplicationFocus(bool state)
  {
    //Debug.Log("app focus : "+focus);
    if(!state && !canUnpause())
    {
      Debug.Log("can't OnApplicationFocus(" + state + ") on focus = " + state);
      return;
    }

    if(EngineEventSystem.onFocusEvent != null) EngineEventSystem.onFocusEvent(state);
  }

  private void OnApplicationPause(bool state)
  {
    //Debug.Log("app pause : "+pause);
    //Debug.Log("app focus : "+focus);
    if (!state && !canUnpause())
    {
      Debug.Log("can't OnApplicationPause("+state+") on focus = " + state);
      return;
    }

    if (EngineEventSystem.onPauseEvent != null) EngineEventSystem.onPauseEvent(state);
  }

  virtual protected bool canUnpause()
  {
    //can't unpause when pause menu is visible
    ScreenObject screen = ScreensManager.getScreen(ScreensManager.ScreenNames.pause);
    //ScreenPause sp = screen as ScreenPause;
    if (screen == null) return true;

    if (screen.name.Contains("pause"))
    {
      if (screen.isVisible()) return false;
    }

    return true;
  }

}
