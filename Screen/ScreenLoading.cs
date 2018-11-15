using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLoading : ScreenObject {

  /// <summary>
  /// force the instance creation of loading screen (if setup)
  /// </summary>
  [RuntimeInitializeOnLoadMethod]
  static public void riolm_loading()
  {
    if (!EngineLoader.isContextEngineCompatible()) return;

    qh.cr<EngineLoadingScreenFeeder>("?loading", true);
  }

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
    _instance.forceHide();
  }
  
}
