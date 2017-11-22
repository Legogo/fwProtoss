using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// pop pop !
/// </summary>

public class EngineManager : MonoBehaviour {
  
  static protected bool state_live = false;
  static protected bool state_loading = true;
  static protected int loadedCount = 0;

  //something need to subscribe to this to get end of loading callback
  public Action onLoadingDone;
  public Action<bool> onPause;

  public int targetFramerate = -1;

  void Awake()
  {
    _manager = this;

    if(targetFramerate > 0)
    {
      Application.targetFrameRate = targetFramerate;
    }

    //new CheckInternet();

    state_loading = true;
    state_live = false;
  }

  public void engine_scenes_loaded()
  {
    //Debug.Log("EngineManager, engine_scenes_loaded, calling all callbacks for end of loading");

    int count = EngineObject.eos.Count;

    for (int i = 0; i < EngineObject.eos.Count; i++)
    {
      //Debug.Log(EngineObject.eos[i].name+" "+EngineObject.eos[i].GetType()+" calling scene loaded");

      EngineObject.eos[i].onEngineSceneLoaded();
    }

    int diff = count - EngineObject.eos.Count;
    if (diff != 0) Debug.LogError("EngineObject :: count diff "+diff+", count changed !");
  }

  public void game_loading_done()
  {
    //Debug.Log("EngineManager, game_loading_done");

    state_loading = false;
    state_live = true;

    if (onLoadingDone != null) onLoadingDone();
  }

  void Update()
  {
    if (Input.GetKeyUp(KeyCode.Space)) // pause
    {
      callPause(state_live);
    }

    if (!isLive()) return;

    //update everything

    List<EngineObject> objects = EngineObject.eos;

    //processUpdateObjectsDebug(objects);
    processUpdateObjects(objects);

    //late
    for (int i = 0; i < objects.Count; i++)
    {
      if (!objects[i].canUpdate()) continue;
      objects[i].updateEngineLate();
    }
  }

  void processUpdateObjectsDebug(List<EngineObject> objects)
  {
    Debug.Log("UBER update (" + objects.Count+")");

    string updateData = "";
    string updateDataFilter = "timer";
    bool addToData = false;

    bool canUpdate = false;
    int count = 0;

    for (int i = 0; i < objects.Count; i++)
    {
      canUpdate = objects[i].canUpdate();
      
      addToData = true;
      if (updateDataFilter.Length > 0 && !objects[i].name.Contains(updateDataFilter)) addToData = false;

      if(addToData){
        updateData += "\n" + objects[i].GetType() + " | " + objects[i].name;
        if (canUpdate) updateData += " update ? <color=green>" + canUpdate + "</color>";
        else updateData += " update ? <color=red>" + canUpdate + "</color>";
      }
      
      if (!canUpdate) continue;

      objects[i].updateEngine();
      count++;
    }

    Debug.Log("updated " + count + " objects");
    Debug.Log(updateData);

  }

  void processUpdateObjects(List<EngineObject> objects)
  {
    
    bool canUpdate = false;
    int count = 0;

    for (int i = 0; i < objects.Count; i++)
    {
      canUpdate = objects[i].canUpdate();
      
      if (!canUpdate) continue;

      objects[i].updateEngine();
      count++;
    }
    
  }

  static public void callPause(bool pauseState)
  {
    Debug.Log("!!system!! callPause(" + pauseState + ")");
    state_live = !pauseState;

    if(_manager != null)
    {
      if (_manager.onPause != null) _manager.onPause(!state_live);
    }
    
  }

  static public bool isLoading(){ return state_loading; }
  static public bool isLive(){return state_live && !state_loading;}

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
