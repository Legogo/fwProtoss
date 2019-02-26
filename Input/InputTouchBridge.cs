using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace fwp.input
{
  public enum BehaviorTargetPlatform { DESKTOP, MOBILE };

  public class InputTouchBridge : MonoBehaviour
  {
    InputTouchPinch pinchBridge;
    InputTouchSwipe swipeBridge;
    InputSelectionManager selectionBridge;
    HelperScreenTouchSequenceSolver sequencer;
    
    public bool useMainCamera = true;
    public Camera inputCamera;
    public LayerMask _layer;

    [Header("pinch|scroll")]
    public float mouseScrollMulFactor = 1f;
    public Vector2 scrollClampMagnitude = Vector2.zero;

    [Header("swipe")]
    public float limitLifeTime = 0.5f;
    public float limitSwipeAmplitude = 200f;
    
    protected List<InputTouchFinger> _fingers = new List<InputTouchFinger>();

    public Action<InputTouchFinger> onTouch;
    public Action<InputTouchFinger> onRelease;
    public Action<InputTouchFinger> onOverring;
    public Action onTouching;

    private int touchCount = 0; // read only

    void Awake()
    {
      manager = this;

      DontDestroyOnLoad(gameObject);

      enabled = false;

      StartCoroutine(processSetup());

      pinchBridge = new InputTouchPinch(this);
      swipeBridge = new InputTouchSwipe(this);
      selectionBridge = new InputSelectionManager();

      if (useDebugInBuild)
      {
        sequencer = new HelperScreenTouchSequenceSolver(new Rect[]{
          new Rect(0.9f, 0.9f, 0.1f, 0.1f),
          new Rect(0.9f, 0.9f, 0.1f, 0.1f)
        });

        sequencer.onToggle += delegate ()
        {
          Debug.Log(getStamp() + "toggling drawDebug : " + drawDebug);
          drawDebug = !drawDebug;
        };
      }

    }

    IEnumerator processSetup()
    {
      if (inputCamera == null)
      {
        while (inputCamera == null)
        {
          fetchCamera();
          yield return null;
        }
        Debug.Log(GetType() + " camera " + inputCamera.name + " is setup", inputCamera.gameObject);
      }

      int qtyFingers = 10;
      if (!isMobile()) qtyFingers = 2;

      //create all 11 touches
      for (int i = 0; i < qtyFingers; i++)
      {
        _fingers.Add(new InputTouchFinger());
      }

#if UNITY_EDITOR
      if (drawDebug) Debug.LogWarning("debug drawing for " + GetType() + " is active");
#endif

      //Debug.Log(GetType() + " setup is done, enabling update");

      enabled = true;
    }

    public void subscribeToScroll(Action<float, float> onScroll)
    {
      pinchBridge.onScroll += onScroll;
    }

    public void subscribeToSwipe(Action<Vector2> onSwipeDelta)
    {
      swipeBridge.onSwipeDelta += onSwipeDelta;
    }

    protected void fetchCamera()
    {
      //debugOverlays = GameObject.FindObjectsOfType<DebugWindowSettings>();

      if (useMainCamera && inputCamera == null)
      {
        inputCamera = Camera.main;

#if UNITY_EDITOR
        if (inputCamera == null) Debug.LogWarning(getStamp()+"no MainCamera tagged in context (frame : "+Time.frameCount+")");
#endif

        return;
      }

      if (inputCamera == null)
      {
        inputCamera = transform.GetComponentInChildren<Camera>();
      }

      //camera tagged as 'input'
      if (inputCamera == null)
      {
        Camera[] cams = GameObject.FindObjectsOfType<Camera>();
        for (int i = 0; i < cams.Length; i++)
        {
          if (inputCamera != null) continue;
          if (HalperLayers.isInLayerMask(cams[i].gameObject, _layer))
          {
            inputCamera = cams[i];
            Debug.LogWarning("{InputTouchBridge} found a camera on 'input' layer");
          }
        }
      }

    }

    /// <summary>
    /// never called by logic, meant to give dev something to reset all touch data
    /// </summary>
    public void reset()
    {
      pinchBridge.reset();

      for (int i = 0; i < _fingers.Count; i++)
      {
        _fingers[i].phase = TouchPhase.Ended;
        onRelease(_fingers[i]);
      }
    }

    /// <summary>
    /// will only update when the setup is done
    /// </summary>
    void Update()
    {

      if (isMobile()) update_touch();
      else update_desktop();

      //on vire les infos des doigts en trop
      update_callbacksAndClean();

      if (countFingers() > 0)
      {
        if (onTouching != null) onTouching();
      }
    }

    void update_callbacksAndClean()
    {

      for (int i = 0; i < _fingers.Count; i++)
      {

        //forcekill unused fingers
        if (i >= touchCount)
        {
          killFinger(_fingers[i]);
          continue;
        }

        //forcekill done fingers
        if (_fingers[i].phase == TouchPhase.Ended)
        {
          killFinger(_fingers[i]);
          continue;
        }

        //solve ...
        if (_fingers[i].phase == TouchPhase.Began)
        {
          //Debug.Log("<RainbowInputManager> touch, finger id : " + _fingers[i].fingerId);
          if (onTouch != null) onTouch(_fingers[i]);
        }
        else if (_fingers[i].phase == TouchPhase.Moved || _fingers[i].phase == TouchPhase.Stationary)
        {
          if (onOverring != null) onOverring(_fingers[i]);
        }

      }

    }

    protected void killFinger(InputTouchFinger finger)
    {
      //les doigts qui ne sont plus utilisés
      if (finger.isPhaseEnded())
      {
        //DebugManager.get().addInput("finger ? " + _fingers[i].fingerId);

        if (onRelease != null)
        {
          //Debug.Log("<RainbowInputManager> release, finger id : " + finger.toString());
          onRelease(finger);
        }

        finger.reset(); // set to canceled
      }

    }

    //ON TOUCH DEVICES (MOBILE,TABLET)
    void update_touch()
    {
      touchCount = Input.touchCount;
      Touch[] touches = Input.touches;

      InputTouchFinger _finger;
      for (int i = 0; i < touchCount; i++)
      {
        _finger = getFingerById(touches[i].fingerId);

        //si on trouve pas de doigt on en prend un dispo
        if (_finger == null) _finger = getFirstAvailableFinger();

        _finger.update(touches[i]);
      }

    }

    //ON PC
    void update_desktop()
    {
      bool mouseDown = Input.GetMouseButton(0);

      touchCount = mouseDown ? 1 : 0;

      InputTouchFinger _finger;
      for (int i = 0; i < _fingers.Count; i++)
      {
        _finger = _fingers[i];

        //les doigts en trop
        if (i >= touchCount && _finger.isFingerUsed()) _finger.setEnded();

        //les doigts qui sont encore là (ou nouveaux)
        else if (i < touchCount) _finger.update(i, Input.mousePosition);
      }

      pinchBridge.update(BehaviorTargetPlatform.DESKTOP);
    }

    public bool hasTouchedCollider(Collider[] list)
    {
      for (int i = 0; i < list.Length; i++)
      {
        if (hasTouchedCollider(list[i])) return true;
      }
      return false;
    }

    public bool hasTouchedCollider(Collider col)
    {
      for (int i = 0; i < _fingers.Count; i++)
      {
        if (!_fingers[i].isFingerUsed()) continue;
        if (_fingers[i].hasCollider(col)) return true;
      }
      return false;
    }

    public Camera getInputCamera() { return inputCamera; }

    /* permet de récup un finger pas utilisé */
    protected InputTouchFinger getFirstAvailableFinger()
    {
      for (int i = 0; i < _fingers.Count; i++)
      {
        if (_fingers[i].isPhaseCanceled()) return _fingers[i];
      }
      return null;
    }

    public InputTouchFinger[] getFingers() { return _fingers.ToArray(); }

    public InputTouchFinger getFingerById(int id, bool checkActivity = true)
    {
      for (int i = 0; i < _fingers.Count; i++)
      {
        //on retourne pas les doigts qui ont déjà fini
        //sur mobile la phase "ended" dure 6+ frames ...
        if (checkActivity && !_fingers[i].isFingerUsed()) continue;

        if (_fingers[i].fingerId == id) return _fingers[i];
      }
      return null;
    }

    public int countFingers() { return touchCount; }
    public InputTouchFinger getFingerByIndex(int index) { return _fingers[index]; }

    public bool hasFingers() { return touchCount > 0; }
    public bool hasSelectedObject()
    {
      for (int i = 0; i < _fingers.Count; i++)
      {
        if (!_fingers[i].isFingerUsed()) continue;
        if (_fingers[i].hasTouchedSomething()) return true;
      }
      return false;
    }

    //http://answers.unity3d.com/questions/150690/using-a-bitwise-operator-with-layermask.html
    public bool isColliderInteractive(Collider collider)
    {
      if (collider == null) return false;
      if ((_layer & (1 << collider.gameObject.layer)) > 0) return true;
      return false;
    }

    public string toString()
    {
      string content = "<color=red>[BRIDGE INPUT MANAGER]</color>";
      content += "\nisMobile() ? " + isMobile();

      content += "\ntouchCount   : " + touchCount;
      content += "\nmax fingers  : " + _fingers.Count;
      for (int i = 0; i < _fingers.Count; i++)
      {
        if (_fingers[i].isFingerUsed())
        {
          content += "\n" + _fingers[i].toString();
        }
        else
        {
          content += "\nfinger(" + i + ") is unused";
        }
      }

      return content;
    }

    /// <summary>
    /// return the name
    /// </summary>
    /// <returns></returns>
    public string getTouchedInteractorInfo()
    {
      string info = "";

      InputTouchFinger fing = _fingers[0];

      if (fing == null || fing.touchedObjects.Count == 0) return "\n\nNo touch";

      Collider2D col = fing.touchedObjects[0].collider;

      info += "\n\nTouch info : ";

      info += "\n" + col.name;

      info += "\n";
      return info;
    }

    static public bool isMobile()
    {
      if (Application.platform == RuntimePlatform.Android) return true;
      else if (Application.platform == RuntimePlatform.IPhonePlayer) return true;
      return false;
    }

#if UNITY_EDITOR
    Color gizmoColor = Color.red;
    void OnDrawGizmos()
    {
      Gizmos.color = gizmoColor;
      for (int i = 0; i < _fingers.Count; i++)
      {
        //Gizmos.DrawSphere(fingers[i].position, 0.5f);
        Gizmos.DrawSphere(_fingers[i].worldPosition, 0.1f);
      }
    }
#endif

    [Header("debug stuff")]
    public bool useDebugInBuild = false;

    public bool drawDebug = false;
    public Vector2 viewDimensions = new Vector2(Screen.width, Screen.height);
    public float viewScaleFactor = 1f;

    protected GUIStyle style;
    void OnGUI()
    {
      if (!drawDebug) return;

      string ctx = toString();

      if (style == null)
      {
        style = new GUIStyle(GUI.skin.textArea);
        style.richText = true;
        style.normal.background = Texture2D.whiteTexture;
      }
      style.fontSize = 40;
      //style.normal.background

      //solve scaling

      viewDimensions.x = Screen.width;
      viewDimensions.y = Screen.height;

      //Vector2 dimensions = viewDimensions;
      //dimensions.y = dimensions.x / (Screen.width * 1f / Screen.height * 1f);

      //viewScaleFactor = Mathf.Max(viewScaleFactor, 0.1f);
      //Vector3 dim = new Vector3(Screen.width / (dimensions.x * viewScaleFactor), Screen.height / (dimensions.y * viewScaleFactor), 1);
      //GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, dim);

      Color black = Color.black;
      black.a = 0.8f;
      GUI.backgroundColor = black;

      guiDrawDebugInfo(0, ctx);

      if (pinchBridge != null)
      {
        guiDrawDebugInfo(1, pinchBridge.toString());
      }

      guiDrawDebugInfo(2, selectionBridge.toString());
    }

    protected void guiDrawDebugInfo(int windowIndex, string ctx)
    {
      //GUI.color = Color.red;
      float width = viewDimensions.x * 1f / (3f * 1.05f);
      float gap = 10f;

      //Debug.Log(viewDimensions.x+" , "+ width);

      GUI.Label(new Rect((gap * windowIndex) + (windowIndex * width), 10, width, Screen.height * 0.5f), ctx, style);
    }

    static public InputTouchFinger getDefaultFinger()
    {
      if (manager.countFingers() <= 0) return null;
      return get()._fingers[0];
    }

    static public Vector2 getDefaultFingerScreenPosition()
    {
      InputTouchFinger f = getDefaultFinger();
      if (f != null) return f.screenPosition;
      return Vector2.zero;
    }

    static private string getStamp()
    {
      return "<color=lightblue>InputTB</color> | ";
    }

    static public Vector2 getCursorPosition()
    {
      InputTouchBridge itb = InputTouchBridge.get();

      if(itb != null && itb.countFingers() > 0)
      {
        return getDefaultFinger().screenPosition;
      }

      return Input.mousePosition;
    }

    static protected InputTouchBridge manager;
    static public InputTouchBridge get()
    {

      //will create double if exist in later scenes (loading)
      //if (manager == null) manager = HalperComponentsGenerics.getManager<InputTouchBridge>("[input]");

      return manager;

    }

  }
}