using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Best setup is to put the input target camera on the input layer (_layer)
/// </summary>

public class InputTouchBridge : MonoBehaviour
{
  //TWEAKABLE
  public float deltaPinch = 0f;

  public bool useMainCamera = false;
  public Camera _camera;
  
  protected List<InputTouchFinger> _fingers = new List<InputTouchFinger>();

  public Action<InputTouchFinger> onTouch; // int is finger ID
  public Action<InputTouchFinger> onRelease;
  public Action<InputTouchFinger> onOverring;

  protected int touchCount = 0; // read only

  //TOOLS
  public LayerMask _layer;

  void Awake() {

    DontDestroyOnLoad(gameObject);

    enabled = false;
    StartCoroutine(processSetup());
  }

  IEnumerator processSetup()
  {
    if(_camera == null)
    {
      while (_camera == null)
      {
        fetchCamera();
        yield return null;
      }
      Debug.Log(GetType()+" camera "+_camera.name+" is setup", _camera.gameObject);
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

    Debug.Log(GetType() + " setup is done, enabling update");

    enabled = true;
  }

  protected void fetchCamera() {
    //debugOverlays = GameObject.FindObjectsOfType<DebugWindowSettings>();

    if (useMainCamera && _camera == null)
    {
      _camera = Camera.main;
      return;
    }

    if (_camera == null)
    {
      _camera = transform.GetComponentInChildren<Camera>();
    }

    //camera tagged as 'input'
    if (_camera == null)
    {
      Camera[] cams = GameObject.FindObjectsOfType<Camera>();
      for (int i = 0; i < cams.Length; i++)
      {
        if (_camera != null) continue;
        if(UnityHelpers.isInLayerMask(cams[i].gameObject, _layer))
        {
          _camera = cams[i];
          Debug.LogWarning("{InputTouchBridge} found a camera on 'input' layer");
        }
      }
    }
    
  }
	
	public void reset(){
    deltaPinch = 0f;

    for (int i = 0; i < _fingers.Count; i++)
    {
      _fingers[i].phase = TouchPhase.Ended;
			onRelease(_fingers[i]);
    }
	}
	
  /// <summary>
  /// will only update when the setup is done
  /// </summary>
	void Update () {
    
    if (isMobile()) update_touch();
    else update_desktop();
    
    //on vire les infos des doigts en trop
    update_callbacksAndClean();
  }
  
  void update_callbacksAndClean() {
    
    for (int i = 0; i < _fingers.Count; i++)
    {

      //forcekill unused fingers
      if (i >= touchCount)
      {
        killFinger(_fingers[i]);
        continue;
      }

      //forcekill done fingers
      if(_fingers[i].phase == TouchPhase.Ended) {
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
  void update_touch() {
    touchCount = Input.touchCount;
    Touch[] touches = Input.touches;

    InputTouchFinger _finger;
    for (int i = 0; i < touchCount; i++)
    {
      _finger = getFingerById(touches[i].fingerId);

      //si on trouve pas de doigt on en prend un dispo
      if(_finger == null) _finger = getFirstAvailableFinger();

      _finger.update(touches[i]);
    }
    
    updatePinch();
  }

  //ON PC
  void update_desktop() {
    bool mouseDown = Input.GetMouseButton(0);
    
    touchCount = mouseDown ? 1 : 0;

    InputTouchFinger _finger;
    for (int i = 0; i < _fingers.Count; i++)
    {
      _finger = _fingers[i];

      //les doigts en trop
      if(i >= touchCount && _finger.isFingerUsed()) _finger.setEnded();

      //les doigts qui sont encore là (ou nouveaux)
      else if(i < touchCount) _finger.update(i, Input.mousePosition);
    }
    
    deltaPinch = Input.mouseScrollDelta.y;
  }

  void updatePinch() {
    if (touchCount == 2)
    {
      // Store both touches.
      InputTouchFinger touchZero = getFingerByIndex(0);
      InputTouchFinger touchOne = getFingerByIndex(1);

      // Find the position in the previous frame of each touch.
      Vector2 touchZeroPrevPos = touchZero.screenPosition - touchZero.screenDeltaPosition;
      Vector2 touchOnePrevPos = touchOne.screenPosition - touchOne.screenDeltaPosition;

      // Find the magnitude of the vector (the distance) between the touches in each frame.
      float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
      float touchDeltaMag = (touchZero.screenPosition - touchOne.screenPosition).magnitude;

      // Find the difference in the distances between each frame.
      deltaPinch = prevTouchDeltaMag - touchDeltaMag;
    }
    else
      deltaPinch = 0f;
  }
  
  public bool hasTouchedCollider(Collider[] list) {
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

  public Camera getInputCamera() { return _camera; }

  /* permet de récup un finger pas utilisé */
  protected InputTouchFinger getFirstAvailableFinger() {
    for (int i = 0; i < _fingers.Count; i++)
    {
      if (_fingers[i].isPhaseCanceled()) return _fingers[i];
    }
    return null;
  }

  public InputTouchFinger[] getFingers(){ return _fingers.ToArray(); }

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
  public InputTouchFinger getFingerByIndex(int index){ return _fingers[index]; }

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
  public bool isColliderInteractive(Collider collider) {
    if (collider == null) return false;
    if ((_layer & (1 << collider.gameObject.layer)) > 0) return true;
    return false;  
  }

  public string toString() {
    string content = "<color=red>[BRIDGE INPUT MANAGER]</color>";
    content += "\nisMobile() ? " + isMobile();
    
    content += "\ntouchCount : " + touchCount;
    content += "\nmax fingers ? " + _fingers.Count;
    for (int i = 0; i < _fingers.Count; i++)
    {
      if(_fingers[i].isFingerUsed()) {
        content += "\n"+_fingers[i].toString();
      }
      else {
        content += "\nfinger("+i+") is unused";
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
  void OnDrawGizmos() {
    Gizmos.color = gizmoColor;
    for (int i = 0; i < _fingers.Count; i++)
    {
      //Gizmos.DrawSphere(fingers[i].position, 0.5f);
      Gizmos.DrawSphere(_fingers[i].worldPosition, 0.1f);
    }
  }

  public bool drawDebug = false;
  protected GUIStyle style = new GUIStyle();
  void OnGUI() {
    if (!drawDebug) return;

    string ctx = toString();

    style.fontSize = Mathf.FloorToInt((Screen.width / Screen.height) * 30f);

    GUI.Label(new Rect(10, 10, 500, 500), ctx, style);
  }

#endif
  
  static protected InputTouchBridge manager;
  static public InputTouchBridge get() {
    if (manager != null) return manager;
    if (manager == null) manager = GameObject.FindObjectOfType<InputTouchBridge>();
    return manager;
  }

}
