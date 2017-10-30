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

  //something need to subscribe to this to get end of loading callback
  public Action onLoadingDone;

  public int targetFramerate = -1;

  void Awake()
  {
    if(targetFramerate > 0)
    {
      Application.targetFrameRate = targetFramerate;
    }

    new CheckInternet();

    state_live = false;
  }

  public void engine_scenes_loaded()
  {
    //Debug.Log("EngineManager, engine_scenes_loaded, calling all callbacks for end of loading");

    for (int i = 0; i < EngineObject.eos.Count; i++)
    {
      EngineObject.eos[i].onEngineSceneLoaded();
    }
  }

  public void game_loading_done()
  {
    //Debug.Log("EngineManager, game_loading_done");

    state_live = true;
    if (onLoadingDone != null) onLoadingDone();
  }

  private void Update()
  {
    if (!isLive()) return;
    
    //update everything

    List<EngineObject> objects = EngineObject.eos;

    //Debug.Log("UBER update (" + objects.Count+")");

    int count = 0;
    //string notUpdatedNames = "";

    for (int i = 0; i < objects.Count; i++)
    {
      if (!objects[i].canUpdate())
      {
        //notUpdatedNames += objects[i].name + " ";
        continue;
      }
      objects[i].updateEngine();
      count++;
    }

    //Debug.Log("updated " + count + " objects");
    //Debug.Log(notUpdatedNames);

    for (int i = 0; i < objects.Count; i++)
    {
      if (!objects[i].canUpdate()) continue;
      objects[i].updateEngineLate();
    }
  }
  
  static public bool isLoading(){return !state_live;}
  static public bool isLive(){return state_live;}

  // au premier launch il faut attendre que tt le monde setup avant de balancer le done()
  static public void checkForStartup() {
    
    if (isLive()) return;

    //everybody setup ?
    loadedCount++;

    if (loadedCount == EngineObject.eos.Count) {
      get().game_loading_done();
    }

  }
  
  public string toStringDebug()
  {
    return name + " live ? " + isLive();
  }
  
  static protected EngineManager _manager;
  static public EngineManager get() {
    if (_manager == null) _manager = GameObject.FindObjectOfType<EngineManager>();
    return _manager;
  }
}
