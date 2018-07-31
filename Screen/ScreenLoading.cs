using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLoading : ScreenObject {

  Camera cam;

  protected override void build()
  {
    base.build();

    sticky = true;
    
    cam = GetComponent<Camera>();
    if (cam == null) cam = GetComponentInChildren<Camera>();

    show();
  }

  /// <summary>
  /// specific call for hide to avoid 'sticky' logic
  /// </summary>
  public void hideScreen()
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
