using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EngineManager : MonoBehaviour {

  static public EngineManager manager;

  public bool state_live = false;

  public Action onLoadingDone;

  void Awake()
  {
    manager = this;

    state_live = false;
  }

  public void game_loading_done()
  {
    state_live = true;
    if (onLoadingDone != null) onLoadingDone();
  }

  static public bool isLive()
  {
    if (manager == null) return false;

    return manager.state_live;
  }
}
