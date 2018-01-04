using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class EngineObject : MonoBehaviour, Interfaces.IDebugSelection
{
  //loading list
  static public List<EngineObject> eos = new List<EngineObject>();

  protected EngineManager _eManager;

  //static int loadingFrameCount = 10;

  protected Transform _tr;
  protected bool _freeze = false;
  protected bool _ready = false;

  [HideInInspector]public ModuleVisible visibility;
  protected InputObject input;

  //constructor
  void Awake()
  {
    _tr = transform;

    _ready = false;
    eos.Add(this);

    //Debug.Log(name);
    //if(name.Contains("ui_")) Debug.Log(name+" <b>added to eos</b>");

    build();
  }

  void Start() {
    
    //usually objects startup their dependencies in the onEngineSceneLoaded
    //so if the object as other monobehavior generated at the same time (same Resource object) engine needs a frame to have all dependencies finish their build() process
    
    if (!EngineManager.isLoading()) onEngineSceneLoaded();
  }

  virtual protected void build()
  {
    fetchLocal();
  }

  /* fetch something on the current object */
  virtual protected void fetchLocal()
  {

  }

  /* fetch something in dependencies that are now ready to be fetched */
  virtual protected void fetchGlobal() {
    visibility = GetComponent<ModuleVisible>();
  }

  protected void subscribeToInput(string carryName) {
    GameObject carry = GameObject.Find(carryName);
    if(carry != null) {
      InputObject io = carry.GetComponent<InputObject>();
      if(io != null) {
        subscribeToInput(io);
        return;
      }
    }

    Debug.LogWarning("asking for inputobject carry " + carryName + " but couldn't find it");
    subscribeToInput();
  }
  protected void subscribeToInput(InputObject io = null)
  {
    if(io != null) {
      input = io;
    }
    else {
      input = GetComponent<InputObject>();
      if (input == null) input = gameObject.AddComponent<InputObject>();
    }

    //Debug.Log(input.name, input.gameObject);

    input.cbTouch += touchPress;
    input.cbRelease += touchRelease;
  }

  virtual protected void touchRelease(InputTouchFinger finger) {

  }

  virtual protected void touchPress(InputTouchFinger finger)
  {

  }

  //called by loader
  virtual public void onEngineSceneLoaded()
  {
    fetchGlobal();
    _ready = true;
  }
  
  /* called by EngineManager */
  virtual public void updateEngine(){}
  virtual public void updateEngineLate(){}

  virtual public bool canUpdate()
  {
    if (isFreezed()) return false;
    return true;
  }
  
  private void OnDestroy()
  {
    //Debug.Log(name + " OnDestroy ", gameObject);
    destroy();
  }

  virtual protected void destroy()
  {
    //Debug.Log(name + " destroy() ", gameObject);
    if (eos.IndexOf(this) > -1) eos.Remove(this);
  }

  public bool isFreezed() { return _freeze; }
  public void setFreeze(bool flag) {_freeze = flag;}
  public bool isReady() { return _ready; }

  public void forceWithinBounds(Rect boundsClamp)
  {
    if (visibility == null) Debug.LogWarning("asking for bounds clamping but no visible module");

    Rect localRec = visibility.getWorldBounds();
    float gap = 0f;

    //Debug.Log(localRec);
    //Debug.Log(boundsClamp);

    gap = boundsClamp.xMax - localRec.xMax;
    if (gap < 0f) transform.position += Vector3.right * gap;

    gap = boundsClamp.xMin - localRec.xMin;
    //Debug.Log(boundsClamp.xMin + " - " + localRec.xMin + " = xmin " + gap);
    if (gap > 0f) transform.position += Vector3.right * gap;

    gap = boundsClamp.yMax - localRec.yMax;
    //Debug.Log(boundsClamp.yMax+" - "+localRec.yMax+" = ymax " + gap);
    if (gap < 0f) transform.position += Vector3.up * gap;

    gap = boundsClamp.yMin - localRec.yMin;
    //Debug.Log(boundsClamp.yMin + " - " + localRec.yMin + " = ymin " + gap);
    if (gap > 0f) transform.position += Vector3.up * gap;
    
  }

  virtual public string toString()
  {
    return name + " freeze ? " + isFreezed()+" canUpdate("+canUpdate()+")";
  }

  public string toStringDebug()
  {
    return toString();
  }
}
