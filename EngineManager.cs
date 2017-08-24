using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// pop pop !
/// </summary>

public class EngineManager : MonoBehaviour {
  
  static protected bool state_live = false;

  public Action onLoadingDone;

  void Awake()
  {
    state_live = false;
  }

  public void game_loading_done()
  {
    state_live = true;
    if (onLoadingDone != null) onLoadingDone();
  }

  static public bool isLive()
  {
    return state_live;
  }

  static protected EngineManager _manager;
  static public EngineManager get() { if (_manager == null) _manager = GameObject.FindObjectOfType<EngineManager>(); return _manager; }
}
