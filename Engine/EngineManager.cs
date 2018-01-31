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

  [Header("system")]
  public int application_targetFramerate = -1;
  public bool log_device_info = true;

  [Header("sound")]
  public DataSounds sounds;

  [Header("logs")]
  public bool mobile_logs_preset = false;
  public StackTraceLogType normal = StackTraceLogType.ScriptOnly;
  public StackTraceLogType warning = StackTraceLogType.ScriptOnly;
  public StackTraceLogType error = StackTraceLogType.ScriptOnly;
  
  void Awake()
  {
    _manager = this;

    if(application_targetFramerate > 0)
    {
      Debug.LogWarning("~EngineManager~ overriding target framerate to " + application_targetFramerate);
      Application.targetFrameRate = application_targetFramerate;
    }
    
    state_loading = true;
    state_live = false;

    EngineLoader.get().onLoadingDone += engine_scenes_loaded;
  }

  /* end of scenes loading */
  public void engine_scenes_loaded()
  {
    ResourceManager.reload();

    //Debug.Log("EngineManager, engine_scenes_loaded, calling all callbacks for end of loading");
    StartCoroutine(processScenesLoaded());
  }

  IEnumerator processScenesLoaded() {
    
    int count = 0;

    //Debug.Log("scenes loaded eos count prev ? "+EngineObject.eos.Count);

    while(count < EngineObject.eos.Count)
    {
      EngineObject.eos[count].onEngineSceneLoaded();

      while (!EngineObject.eos[count].isReady()) yield return null;

      count++;
      yield return null;
    }

    //Debug.Log("scenes loaded eos count after ? " + EngineObject.eos.Count);

    game_loading_done();
  }

  public void game_loading_done()
  {
    //Debug.Log("EngineManager, game_loading_done");

    state_loading = false;
    state_live = true;

    //broadcast
    if (onLoadingDone != null) onLoadingDone();
  }

  void Update()
  {
    if (Input.GetKeyUp(KeyCode.P)) // pause
    {
      callPause(state_live);
    }

    //exit app
    if (Input.GetKeyUp(KeyCode.Delete) || Input.GetKeyUp(KeyCode.Backspace))
    {
      Application.Quit();
      return;
    }

    if (!isLive()) return;

    GameTime.update();

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
    Debug.Log("##system## callPause(" + pauseState + ")");
    state_live = !pauseState;

    if(_manager != null)
    {
      if (_manager.onPause != null) _manager.onPause(!state_live);
    }
    
  }

  static public bool isLoading(){ return state_loading; }
  static public bool isLive(){return state_live && !state_loading;}
  
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
