using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Manager that will cal updates for all EngineObject(s)
/// Everything starts when onLoadingDone is called (after engine scenes additive loading)
/// </summary>

public class EngineManager : MonoBehaviour {
  
  static public void create()
  {
    EngineManager em = GameObject.FindObjectOfType<EngineManager>();
    if(em == null)
    {
      em = new GameObject("[engine]").AddComponent<EngineManager>();
      Debug.LogWarning("engine manager wasn't loaded or is non existent. creating one");
    }
  }

  static public Dictionary<int, List<EngineObject>> eosLayers;

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

  static public void subscribe(EngineObject obj)
  {
    if (eosLayers == null) eosLayers = new Dictionary<int, List<EngineObject>>();

    if (!eosLayers.ContainsKey(obj.engineLayer)) eosLayers.Add(obj.engineLayer, new List<EngineObject>());
    eosLayers[obj.engineLayer].Add(obj);

    //Debug.Log(obj.name + " added to eos on layer "+obj.engineLayer);
  }
  static public void unsubscribe(EngineObject obj)
  {
    if (eosLayers == null) return;
    eosLayers[obj.engineLayer].Remove(obj);
  }

  /* end of scenes loading */
  public void engine_scenes_loaded()
  {
    ResourceManager.reload();

    //Debug.Log("EngineManager, engine_scenes_loaded, calling all callbacks for end of loading");
    //StartCoroutine(processScenesLoaded());
    processScenesLoaded();
  }

  /* telle everybody present that all scenes are now loaded */
  void processScenesLoaded() {
    
    //Debug.Log("~EngineManager~ <b>scenes loaded</b> | layer count ? " + eosLayers.Count);

    for (int i = 0; i < eosLayers.Count; i++)
    {
      List<EngineObject> list = eosLayers[i];

      //Debug.Log("end of loading for layer " + i+" | objects count ? "+list.Count);

      //int count = list.Count;

      int idx = 0;
      while (idx < list.Count)
      {
        //might add objects to layer list during loop
        list[idx].onEngineSceneLoaded();
        idx++;

        //object was destroy on end loading ? next
        //if (list[idx] == null) list.RemoveAt(idx);
        //else idx++;
      }

      //more objects added during loading
      //if (count != list.Count) Debug.LogError("something changed");
    }

    Debug.Log("~EngineManager~ <color=yellow># SCENES LOADED #</color>");
    
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
    
    //update everything
    
    foreach(KeyValuePair<int, List<EngineObject>> layer in eosLayers)
    {
      for (int i = 0; i < layer.Value.Count; i++)
      {
        layer.Value[i].updateEngine();
      }
      for (int i = 0; i < layer.Value.Count; i++)
      {
        layer.Value[i].updateEngineLate();
      }
    }
    
  }

  private void FixedUpdate()
  {
    GameTime.update();
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
