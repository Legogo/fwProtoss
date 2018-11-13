﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLoading : ScreenObject {

  /// <summary>
  /// force the instance creation of loading screen (if setup)
  /// </summary>
  [RuntimeInitializeOnLoadMethod]
  static public void riolm_loading()
  {
    qh.cr<EngineLoadingScreenFeeder>("?loading", true);
  }

  Camera cam;

  protected override void build()
  {
    base.build();

    //loading must be sticky to not be closed by maanger open/close logic
    //of other screens
    if(!sticky) sticky = true;
    
    cam = GetComponent<Camera>();
    if (cam == null) cam = GetComponentInChildren<Camera>();

    show();
  }
  
  static public void hideLoadingScreen()
  {
    get().forceHide();
  }

  static protected ScreenLoading instance;
  static public ScreenLoading get()
  {
    if (instance == null) instance = GameObject.FindObjectOfType<ScreenLoading>();
    return instance;
  }
}
