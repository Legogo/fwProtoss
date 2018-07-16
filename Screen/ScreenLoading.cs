using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLoading : ScreenObject {

  Camera cam;

  protected override void build()
  {
    base.build();
    
    cam = GetComponent<Camera>();
    show();
  }
  
  /// <summary>
  /// needs to be called by game loading end event
  /// </summary>
  public void hideLoadingScreen()
  {
    toggleVisible(false);
    cam.enabled = false;
  }

  static protected ScreenLoading instance;
  static public ScreenLoading get()
  {
    if (instance == null) instance = GameObject.FindObjectOfType<ScreenLoading>();
    return instance;
  }
}
