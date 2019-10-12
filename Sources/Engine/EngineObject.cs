using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using fwp.input;

#if UNITY_EDITOR
using UnityEditor;
#endif

abstract public class EngineObject : MonoBehaviour
{
  protected EngineManager _eManager;

  [HideInInspector]
  public int engineLayer = 0; // script execution order

  protected Transform _eoTransform;
  protected bool _unfreeze = true;
  protected bool _ready = false;
  
  [Serializable]
  public enum VisibilityMode { NONE, SPRITE, UI, MESH, SKINNED };
  public HelperVisible visibility;

  //[Serializable]public enum InputMode { NONE, MOUSE };
  //public InputMode inputMode;

  protected bool logs = false; // display logs
  protected HelperInputObject inputObject = null;
  protected bool debugActiveScene = false;

  //constructor
  private void Awake()
  {
    _eoTransform = transform;

    _ready = false;

    //Debug.Log(name);
    //if(name.Contains("hopper")) Debug.Log("build <b>"+name+"</b>");

    debugActiveScene = HalperScene.isActiveScene(gameObject.scene.name);

    build();
  }

  private IEnumerator Start() {

    //si le manager recoit l'event de fin de loading après que Start soit exec
    //yield return null;

    //usually objects startup their dependencies in the onEngineSceneLoaded
    //so if the object as other monobehavior generated at the same time (same Resource object) engine needs a frame to have all dependencies finish their build() process

    //Debug.Log(GetType() + " <b>" + name + "</b> START", gameObject);

    if (EngineStartup.compatibility)
    {
      //attendre que le l'engine ai démarré
      //qui doit etre contenu dans resource-engine.scene
      while (EngineStartup.instanceExist()) yield return null;
      while (EngineManager.get() == null) yield return null;
      while (EngineManager.isLoading()) yield return null;
    }

    //si le manager recoit l'event de fin de loading après que Start soit exec
    //il y aura un UPDATE de l'engine avant de repasser par ici et de setup l'objet
    //while (EngineManager.isLoading()) yield return null;

    setupEarly();
    
    yield return null;

    setup();

    yield return null;

    setupLate();
    
    if(debugActiveScene) setupDebug();
    
    _ready = true; // all setup done, can now update
  }

  protected void overrideEngineLayer(int newLayer)
  {
    if (_ready)
    {
      Debug.LogError("you can't switch layer after constructor");
      return;
    }
    engineLayer = newLayer;
  }
  
  virtual protected void build()
  {
    buildVisibilty();
  }

  protected void buildVisibilty()
  {
    //Debug.Log(name + " build visib");

    VisibilityMode mode = getVisibilityType();
    
    switch (mode)
    {
      case VisibilityMode.SPRITE: visibility = new HelperVisibleSprite(this);break;
      case VisibilityMode.MESH: visibility = new HelperVisibleMesh(this); break;
      case VisibilityMode.UI: visibility = new HelperVisibleUi(this); break;
      case VisibilityMode.SKINNED: visibility = new HelperVisibleSkinned(this); break;
      case VisibilityMode.NONE: break;
      default: Debug.LogError("this visibilty mode ("+ mode.ToString()+") is not implem yet"); break;
    }
  }
  
  /// <summary>
  /// to override if need a specific visibility helper mode
  /// </summary>
  /// <returns></returns>
  virtual protected VisibilityMode getVisibilityType() { return VisibilityMode.NONE; }

  protected HelperInputObject subscribeToInput(string carryName = "")
  {
    if (inputObject != null) return inputObject;

    HelperInputObject hio = null;

    if (carryName.Length > 0)
    {
      GameObject carry = GameObject.Find(carryName);
      if (carry != null)
      {
        EngineObject eo = carry.GetComponent<EngineObject>();
        hio = eo.inputObject;
      }
      else
      {
        Debug.LogWarning("asking for inputobject carry " + carryName + " but couldn't find it");
      }
    }

    if (hio == null)
    {
      hio = inputObject;
    }

    if(hio == null)
    {
      hio = new HelperInputObject(this);
    }

    inputObject = hio;

    return inputObject;
  }

  /// <summary>
  /// subscribe touch() & release() callbacks to <InputObject>, carryName can be empty to use/create attached <InputObject>
  /// </summary>
  protected HelperInputObject subscribeToTouchRelease(Action<InputTouchFinger> touch, Action<InputTouchFinger> release = null) {
    HelperInputObject hio = subscribeToInput();
    inputObject.cbTouch += touch;
    inputObject.cbRelease += release;
    return hio;
  }

  protected HelperInputObject subscribeToTouchReleaseOver(Action<InputTouchFinger, RaycastHit2D> touchOver, Action<InputTouchFinger, RaycastHit2D> releaseOver = null) {
    HelperInputObject hio = subscribeToInput();
    inputObject.cbTouchOver += touchOver;
    inputObject.cbReleaseOver += releaseOver;
    return hio;
  }
  
  /* how this object will create some stuff before setup-ing (ie : symbols) */
  virtual protected void setupEarly()
  { }

  /* called by onEngineSceneLoaded, fetch something in dependencies that are now ready to be fetched */
  virtual protected void setup()
  {
    //Debug.Log("fetching global <b>" + name + "</b> (layer " + engineLayer + ") | visibility ? "+visibility, gameObject);
    if (visibility != null) visibility.setup();

    //link to engine to be updated
    EngineManager.subscribe(this); // to have updates functions working
  }

  virtual protected void setupLate()
  { }

  /// <summary>
  /// called after late if active scene is owner scene
  /// </summary>
  virtual protected void setupDebug()
  { }

  /// <summary>
  /// you should NOT override this function
  /// </summary>
  void Update()
  {
    if(!EngineStartup.compatibility)
    {
      updateEngine();
      updateEngineLate();
    }
  }

  /* called by EngineManager */
  virtual public void updateEngine() { }
  virtual public void updateEngineLate() { }

  virtual public bool canUpdate()
  {
    if (!enabled) return false; // only if changed in editor, no Update() here
    if (!gameObject.activeSelf) return false; // specific cases

    if (!_ready) return false; // loading (true after setupLate)
    if (isFreezed()) return false; // freeze logic

    return true;
  }

  public bool isFreezed() { return !_unfreeze; }
  public void setFreeze(bool flag) { _unfreeze = !flag; } // unfreeze true == ça tourne !
  public bool isReady() { return _ready; }

  virtual public string toString()
  {
    string ct = name+" ["+GetType()+"]";
    ct += "\n └ " + iStringFormatBool("UNfreezed", _unfreeze);
    ct += "\n └ " + iStringFormatBool("can update", canUpdate());
    ct += "\n └ " + iStringFormatBool("ready", _ready);
    ct += enabled + " , " + gameObject.activeSelf + " , " + _ready + " , " + _unfreeze;
    return ct;
  }

  protected string iStringFormatBool(string label, bool val)
  {
    return label + " ? " + (val ? "<color=green>true</color>" : "<color=red><b>false</b></color>");
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

  /// <summary>
  /// This function is meant to keep Z order
  /// </summary>
  /// <param name="newPosition"></param>
  /// <returns></returns>
  public Vector3 setXYPosition(Vector2 newPosition)
  {
    Vector3 pos = transform.position;
    pos.x = newPosition.x;
    pos.y = newPosition.y;
    //keep z
    transform.position = pos;
    return pos;
  }

  public HelperInputObject getIO()
  {
    return inputObject;
  }

  public string toDebug()
  {
    return toString();
  }

  virtual protected string getStamp()
  {
    return getStamp(this);
  }

  static public string getStamp(MonoBehaviour obj, string color = "gray")
  {
    return "<color=" + color + ">" + obj.GetType() + "</color> | <b>" + obj.name + "</b> | ";
  }

  /// <summary>
  /// editor selection
  /// </summary>
  /// <returns></returns>
  public bool isSelection()
  {
#if UNITY_EDITOR
    return UnityEditor.Selection.activeGameObject == gameObject;
#else
    return false;
#endif
  }

  public void log(string data, Transform logTarget = null)
  {
    if (!logs) return;
    if (logTarget == null) logTarget = transform;
    Debug.Log(GetType() + " | " + data, logTarget);
  }
  
  protected void setAsEditorSelection(GameObject obj = null, bool parentIsActiveSceneCheck = false)
  {
#if UNITY_EDITOR
    if (parentIsActiveSceneCheck && !debugActiveScene) return;
    if (obj == null) obj = gameObject;
    UnityEditor.Selection.activeGameObject = obj;
#endif
  }





  // DEBUG DRAW

  protected void drawCircle(Vector3 position, float radius, Color color, float alpha = 1f, string label = "")
  {
#if UNITY_EDITOR
    color.a = alpha;

    Gizmos.DrawLine(position, position + Vector3.up * (radius + 0.05f));

    Gizmos.color = color;
    Gizmos.DrawSphere(position, radius);

    if (label.Length > 0) Handles.Label(position + (Vector3.right * 0.01f) + Vector3.up * (radius + 0.05f), label);
#endif
  }

  protected void drawCross(Vector3 position, float size, Color color, string label = "")
  {
#if UNITY_EDITOR
    Gizmos.color = color;
    Gizmos.DrawLine(transform.position + Vector3.up * size, transform.position + Vector3.down * size);
    Gizmos.DrawLine(transform.position + Vector3.left * size, transform.position + Vector3.right * size);

    if (label.Length > 0) Handles.Label(transform.position, name);
#endif
  }
}
