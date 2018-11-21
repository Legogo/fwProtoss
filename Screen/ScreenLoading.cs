using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// to call loading screen before everything else put <EngineLoadingScreenFeeder> in startup scene
/// </summary>

public class ScreenLoading : ScreenObject {
  
  static protected ScreenLoading _instance;

  Camera cam;

  protected override void build()
  {
    base.build();

    _instance = this;

    //loading must be sticky to not be closed by maanger open/close logic
    //of other screens
    if (!sticky) sticky = true;
    
    cam = GetComponent<Camera>();
    if (cam == null) cam = GetComponentInChildren<Camera>();

    //Debug.Log("hiding loading screen through static call");
    show();
  }
  
  static public void hideLoadingScreen()
  {
    Debug.Log("hiding loading screen through static call");

    if(_instance == null)
    {
      Debug.LogWarning("asking to hide loading but instance is null ?");
      return;
    }

    _instance.forceHide();
  }
  
}
