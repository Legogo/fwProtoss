using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// pop pop !
/// </summary>

public class EngineManager : MonoBehaviour {
  
  static protected bool state_live = false;
  static protected int loadedCount = 0;

  public Action onLoadingDone;

  public int targetFramerate = -1;

  void Awake()
  {
    if(targetFramerate > 0)
    {
      Application.targetFrameRate = targetFramerate;
    }
    
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

    List<EngineObject> objects = EngineObject.eos;
    for (int i = 0; i < objects.Count; i++)
    {
      if (objects[i].isFreezed()) continue;
      objects[i].update();
    }
  }

  static public bool isLive()
  {
    return state_live;
  }

  // au premier launch il faut attendre que tt le monde setup avant de balancer le done()
  static public void checkForStartup() {
    
    if (isLive()) return;

    loadedCount++;

    if (loadedCount == EngineObject.eos.Count) {
      get().game_loading_done();
    }

  }

  static protected EngineManager _manager;
  static public EngineManager get() {
    if (_manager == null) _manager = GameObject.FindObjectOfType<EngineManager>();
    return _manager;
  }
}
