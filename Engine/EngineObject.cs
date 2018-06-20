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
  protected bool _unfreeze = true;
  protected bool _ready = false;
  
  [Serializable]public enum VisibilityMode { NONE, SPRITE, UI, MESH, SKINNED };
  public VisibilityMode visibilityMode;
  public HelperVisible visibility;
  
  //[Serializable]public enum InputMode { NONE, MOUSE };
  //public InputMode inputMode;

  private InputObject inputObject;

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
    //Debug.Log(name + " build visib");

    switch (visibilityMode)
    {
      case VisibilityMode.SPRITE: visibility = new HelperVisibleSprite(this);break;
      case VisibilityMode.MESH: visibility = new HelperVisibleMesh(this); break;
      case VisibilityMode.UI: visibility = new HelperVisibleUi(this); break;
      case VisibilityMode.SKINNED: visibility = new HelperVisibleSkinned(this); break;
      case VisibilityMode.NONE: break;
      default: Debug.LogError("this visibilty mode ("+visibilityMode.ToString()+") is not implem yet"); break;
    }
  }

  /// <summary>
  /// subscribe touch() & release() callbacks to <InputObject>, carryName can be empty to use/create attached <InputObject>
  /// </summary>
  protected void subscribeToInput(Action<InputTouchFinger> touch, Action<InputTouchFinger> release = null, string carryName = "") {
    if(carryName.Length > 0)
    {
      GameObject carry = GameObject.Find(carryName);
      if (carry != null)
      {
        InputObject io = carry.GetComponent<InputObject>();
        if (io != null)
        {
          subscribeToInput(io, touch, release);
          return;
        }
      }

      Debug.LogWarning("asking for inputobject carry " + carryName + " but couldn't find it");
    }

    subscribeToInput(null, touch, release);
  }

  private void subscribeToInput(InputObject io, Action<InputTouchFinger> touch, Action<InputTouchFinger> release)
  {
    if (io != null) {
      inputObject = io;
    }
    else {
      inputObject = GetComponent<InputObject>();
      if (inputObject == null) inputObject = gameObject.AddComponent<InputObject>();
    }

    //Debug.Log(input.name, input.gameObject);

    inputObject.cbTouch += touch;
    inputObject.cbRelease += release;
  }
  
  //called by loader (twice for early and setup)
  public void onEngineSceneLoaded()
  {
    //Debug.Log(GetType()+" , <b>"+name+ "</b> onEngineSceneLoaded", gameObject);

    if (!_ready)
    {
      setupEarly();
      _ready = true;
      return;
    }

    setup();
  }

  /* how this object will create some stuff before setup-ing (ie : symbols) */
  virtual protected void setupEarly()
  {

  }

  /* called by onEngineSceneLoaded, fetch something in dependencies that are now ready to be fetched */
  virtual protected void setup()
  {
    //Debug.Log("fetching global <b>" + name + "</b> (layer " + engineLayer + ") | visibility ? "+visibility, gameObject);
    if (visibility != null) visibility.setup();
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

  public bool isFreezed() { return !_unfreeze; }
  public void setFreeze(bool flag) { _unfreeze = !flag; }
  public bool isReady() { return _ready; }

  virtual public string toString()
  {
    return name + "\n └ "+iStringFormatBool("unfreezed", _unfreeze)+"\n └ "+iStringFormatBool("can update", canUpdate());
  }

  protected string iStringFormatBool(string label, bool val)
  {
    return label + " ? " + (val ? "<color=green>true</color>" : "<color=red><b>false</b></color>");
  }

  public string iString()
  {
    return toString();
  }

  
}
