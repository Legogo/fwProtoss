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

  public List<EngineObject> objects = new List<EngineObject>();

  void Awake()
  {
    Application.targetFrameRate = 60;

    state_live = false;
  }

  public void game_loading_done()
  {
    state_live = true;
    if (onLoadingDone != null) onLoadingDone();
  }

  private void Update()
  {
    if (!isLive()) return;

    for (int i = 0; i < objects.Count; i++)
    {
      objects[i].update();
    }
  }

  static public bool isLive()
  {
    return state_live;
  }

  static protected EngineManager _manager;
  static public EngineManager get() {
    if (_manager == null) _manager = GameObject.FindObjectOfType<EngineManager>();
    return _manager;
  }
}
