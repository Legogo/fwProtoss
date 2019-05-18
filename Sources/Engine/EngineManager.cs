using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using fwp.sound;

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

  static private List<EngineObject> eos; // layer 0
  static public SortedDictionary<int, List<EngineObject>> eosNegLayers; // layers ]-inf,-1]
  static public SortedDictionary<int, List<EngineObject>> eosPosLayers; // layers [1,inf[

  static protected bool state_live = false;
  static protected bool state_loading = true;
  static protected int loadedCount = 0;

  public List<EngineObject> lockers = new List<EngineObject>();

  //something need to subscribe to this to get end of loading callback
  public Action onLoadingDone;

  [Header("system")]
  public int application_targetFramerate = -1;
  public bool log_device_info = true;

  [Header("sound")]
  public bool useSoundFramework = false;
  public AudioMixer mixer;

  [Header("mobile specifics")]
  public bool mobile_never_sleep = false;
  
  [Header("logs")]
  public bool mobile_logs_preset = false; // if true none,none,scriptonly on mobile

  public StackTraceLogType normal = StackTraceLogType.ScriptOnly;
  public StackTraceLogType warning = StackTraceLogType.ScriptOnly;
  public StackTraceLogType error = StackTraceLogType.ScriptOnly;

  EngineObject tmpEo = null;

  void Awake()
  {
    _manager = this;

    if(eos == null) eos = new List<EngineObject>();

    //https://docs.unity3d.com/ScriptReference/Application-targetFrameRate.html
    if (application_targetFramerate > 0)
    {
      Debug.LogWarning(getStamp()+"overriding target <b>framerate to " + application_targetFramerate+"</b>");
      Application.targetFrameRate = application_targetFramerate;
      Debug.LogWarning(getStamp() + "removing vsync");
      QualitySettings.vSyncCount = 0;
    }

    Debug.Log(GlobalSettingsSystem.getSystemInfo());

    //https://docs.unity3d.com/ScriptReference/Screen-sleepTimeout.html
    Screen.sleepTimeout = (mobile_never_sleep) ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
    Debug.Log(getStamp() + " sleep timeout is setup to : <b>" + Screen.sleepTimeout + "</b> (-2 = system | -1 = never sleep)");
    
    state_loading = true;
    state_live = false;
    
    if(useSoundFramework) SoundManager.generate(this);
    
    GlobalSettingsSystem.setupTraceLog();

    EngineLoader.get().onLoadingDone += engine_scenes_loaded;
  }

  static public void subscribe(EngineObject obj)
  {
    if (obj.engineLayer == 0)
    {
      eos.Add(obj);
      return;
    }

    //pos
    if(obj.engineLayer < 0)
    {
      if (eosNegLayers == null) eosNegLayers = new SortedDictionary<int, List<EngineObject>>();

      if (!eosNegLayers.ContainsKey(obj.engineLayer)) eosNegLayers.Add(obj.engineLayer, new List<EngineObject>());
      eosNegLayers[obj.engineLayer].Add(obj);
    }

    //neg
    if (obj.engineLayer > 0)
    {
      if (eosPosLayers == null) eosPosLayers = new SortedDictionary<int, List<EngineObject>>();

      if (!eosPosLayers.ContainsKey(obj.engineLayer)) eosPosLayers.Add(obj.engineLayer, new List<EngineObject>());
      eosPosLayers[obj.engineLayer].Add(obj);
    }

    //Debug.Log(obj.name + " added to eos on layer "+obj.engineLayer);
  }
  
  static public void unsubscribe(EngineObject obj)
  {
    if(obj.engineLayer == 0)
    {
      eos.Remove(obj);
      return;
    }

    if (obj.engineLayer < 0) eosNegLayers[obj.engineLayer].Remove(obj);
    if (obj.engineLayer > 0) eosPosLayers[obj.engineLayer].Remove(obj);
  }

  /* end of scenes loading */
  public void engine_scenes_loaded()
  {
    ResourceManager.reload();
    
    Debug.Log(getStamp()+ "engine_scenes_loaded()");

    state_loading = false;
    
    //broadcast
    if (onLoadingDone != null) onLoadingDone();

    state_live = true;
  }

  void Update()
  {
    GameTime.update();

    //if (!isLive()) return;

    if (!state_live) return;
    if (state_loading) return;

    processLayerUpdate();

  }
  
  void processLayerUpdate()
  {
    //update

    if (eosNegLayers != null)
    {
      for (int i = 0; i < eosNegLayers.Count; i++) processUpdateLayer(eosNegLayers[i]);
    }

    if (eos != null) processUpdateLayer(eos);

    if (eosPosLayers != null)
    {
      for (int i = 0; i < eosPosLayers.Count; i++) processUpdateLayer(eosPosLayers[i]);
    }

    //late update

    if (eosNegLayers != null)
    {
      for (int i = 0; i < eosNegLayers.Count; i++) processUpdateLayerLate(eosNegLayers[i]);
    }

    if (eos != null) processUpdateLayerLate(eos);

    if (eosPosLayers != null)
    {
      for (int i = 0; i < eosPosLayers.Count; i++) processUpdateLayerLate(eosPosLayers[i]);
    }

  }

  void processUpdateLayer(List<EngineObject> objects)
  {
    for (int i = 0; i < objects.Count; i++)
    {
      tmpEo = objects[i];
      if (!tmpEo.canUpdate()) continue;
      tmpEo.updateEngine();
    }
  }

  void processUpdateLayerLate(List<EngineObject> objects)
  {
    for (int i = 0; i < objects.Count; i++)
    {
      tmpEo = objects[i];
      if (!tmpEo.canUpdate()) continue;
      tmpEo.updateEngineLate();
    }
  }


  void processUpdateObjectsDebug(List<EngineObject> objects)
  {
    Debug.Log(getStamp()+" UBER update (" + objects.Count+")");

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

      objects[i].updateEngine(); // processUpdateObjectsDebug
      count++;
    }

    Debug.Log(getStamp()+"updated " + count + " objects");
    Debug.Log(updateData);

  }

  /// <summary>
  /// deprecated, see 
  /// </summary>
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
  
  static public void setPause(bool flag) { state_live = flag; }
  static public bool isPaused() { return !state_live; }

  static public bool isLoading(){ return state_loading; }
  static public bool isLive(){ return state_live && !state_loading; }
  
  public string toStringDebug()
  {
    return name + " live ? " + isLive();
  }

  static protected string getStamp()
  {
    return "<color=orange>EngineManager</color> | ";
  }
  
  static protected EngineManager _manager;
  static public EngineManager get() {
    if (_manager == null) _manager = GameObject.FindObjectOfType<EngineManager>();
    return _manager;
  }
}
