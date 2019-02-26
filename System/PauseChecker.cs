using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// meant to be the funnel point to catch a pause event
/// </summary>

abstract public class PauseChecker : MonoBehaviour {
  
  private void OnApplicationFocus(bool state)
  {
    //Debug.Log("app focus : "+focus);
    if(!state && !canUnpause())
    {
      Debug.Log("can't unpause on focus = " + state);
      return;
    }

    if(EngineEventSystem.onFocusEvent != null) EngineEventSystem.onFocusEvent(state);
  }

  private void OnApplicationPause(bool state)
  {
    //Debug.Log("app pause : "+pause);
    if (EngineEventSystem.onPauseEvent != null) EngineEventSystem.onPauseEvent(state);
  }

  abstract protected bool canUnpause();

}
