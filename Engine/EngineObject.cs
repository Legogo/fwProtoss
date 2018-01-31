using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class EngineObject : MonoBehaviour, Interfaces.IDebugSelection
{
  protected EngineManager _eManager;

  [HideInInspector]
  public int engineLayer = 0;

  protected Transform _tr;
  protected bool _freeze = false;
  protected bool _ready = false;
  
  [Serializable]public enum VisibilityMode { NONE, SPRITE, UI, MESH };
  public VisibilityMode visibilityMode;
  public HelperVisible visibility;
  
  //[Serializable]public enum InputMode { NONE, MOUSE };
  //public InputMode inputMode;

  protected InputObject input;

  //constructor
  void Awake()
  {
    _tr = transform;

    _ready = false;

    //Debug.Log(name);
    //if(name.Contains("hopper")) Debug.Log("build <b>"+name+"</b>");

    build();
  }

  protected void overrideLayer(int newLayer)
  {
    if (_ready)
    {
      Debug.LogError("you can't switch layer after constructor");
      return;
    }
    engineLayer = newLayer;
  }

  void Start() {

    //usually objects startup their dependencies in the onEngineSceneLoaded
    //so if the object as other monobehavior generated at the same time (same Resource object) engine needs a frame to have all dependencies finish their build() process

    //Debug.Log(GetType() + " <b>" + name + "</b> START", gameObject);

    if (!EngineManager.isLoading())
    {
      //Debug.Log(GetType() + " <b>" + name + "</b> engine is not loading, sending callback", gameObject);
      onEngineSceneLoaded();
    }
  }

  virtual protected void build()
  {
    buildVisibilty();
    EngineManager.subscribe(this);
  }

  protected void buildVisibilty()
  {
    switch (visibilityMode)
    {
      case VisibilityMode.SPRITE: visibility = new HelperVisibleSprite();break;
      case VisibilityMode.MESH: visibility = new HelperVisibleMesh(); break;
      case VisibilityMode.NONE: break;
      default: Debug.LogError("no implem yet"); break;
    }
  }

  protected void subscribeToInput(string carryName) {
    GameObject carry = GameObject.Find(carryName);
    if (carry != null) {
      InputObject io = carry.GetComponent<InputObject>();
      if (io != null) {
        subscribeToInput(io);
        return;
      }
    }

    Debug.LogWarning("asking for inputobject carry " + carryName + " but couldn't find it");
    subscribeToInput();
  }

  protected void subscribeToInput(InputObject io = null)
  {
    if (io != null) {
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
  public void onEngineSceneLoaded()
  {
    //Debug.Log(GetType()+" , <b>"+name+ "</b> onEngineSceneLoaded", gameObject);

    createGlobal();

    fetchGlobal();

    _ready = true;
  }

  /* how this object will create some stuff before fetching (ie : symbols) */
  virtual protected void createGlobal()
  {

  }

  /* called by onEngineSceneLoaded, fetch something in dependencies that are now ready to be fetched */
  virtual protected void fetchGlobal()
  {
    //Debug.Log("fetching global <b>" + name + "</b> (layer " + engineLayer + ") | visibility ? "+visibility, gameObject);
    if (visibility != null) visibility.setup(this);
  }

  /* called by EngineManager */
  virtual public void updateEngine() { }
  virtual public void updateEngineLate() { }

  virtual public bool canUpdate()
  {
    if (isFreezed()) return false;
    return true;
  }

  private void OnDestroy()
  {
#if UNITY_EDITOR
    //don't clear anything if editor stop playing
    if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
#endif
    //Debug.Log(name + " OnDestroy ", gameObject);
    destroy();
  }

  virtual protected void destroy()
  {
    //Debug.Log(name + " destroy() ", gameObject);
    //if (eos.IndexOf(this) > -1) eos.Remove(this);
    EngineManager.unsubscribe(this);
  }

  public bool isFreezed() { return _freeze; }
  public void setFreeze(bool flag) { _freeze = flag; }
  public bool isReady() { return _ready; }

  virtual public string toString()
  {
    return name + " freeze ? " + isFreezed() + " canUpdate(" + canUpdate() + ")";
  }

  public string toStringDebug()
  {
    return toString();
  }

  
}
