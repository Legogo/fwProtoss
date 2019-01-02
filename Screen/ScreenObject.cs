using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// show,hide
/// updateVisible,updateNotVisible
/// </summary>

public class ScreenObject : EngineObject
{
  protected ArenaManager _arena;

  public ScreensManager.ScreenType type;

  public bool useUiCamera = false;
  public bool sticky = false; // can't be hidden
  public bool dontHideOtherOnShow = false; // won't close other non sticky screen when showing

  protected Canvas mainCanvas;
  protected Canvas[] _canvas;
  protected RectTransform _rt;

  protected float notInteractiveTimer = 0f;

  float moveTimerDelayTime = 0.1f;
  float moveTimerDelay = 0f;

  protected Action onPressedDown;
  protected Action onPressedUp;
  protected Action onPressedLeft;
  protected Action onPressedRight;

  protected override void build()
  {
    base.build();

    _canvas = transform.GetComponentsInChildren<Canvas>();
    if (_canvas == null) Debug.LogError("no canvas ?");

    mainCanvas = getCanvas();

    _rt = GetComponent<RectTransform>();

    //if (_canvas == null) Debug.LogError("wat ?");

    // / ! \
    //hide() will trigger modif of other component that are catched during build()

    //generic behavior, won't work for non sticky screens
    //hide();

  }

  protected override void setupEarly()
  {
    base.setupEarly();
    
    if (useUiCamera)
    {
      Camera uiCam = qh.gc<Camera>("camera-ui");
      if (uiCam == null) Debug.LogError("no camera ui found");
      if(mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
      {
        int sort = mainCanvas.sortingOrder;
        if(sort == 0)
        {
          Debug.LogWarning("when using ui camera sort should be > 0, it's used as plane distance");
        }

        mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        mainCanvas.worldCamera = uiCam;
        mainCanvas.planeDistance = sort; // use sort order for plane distance
      }
    }

    hide();
  }

  public void subscribeToPressedEvents(Action down, Action up, Action left, Action right)
  {
    if (down != null) onPressedDown += down;
    if (up != null) onPressedUp += up;
    if (left != null) onPressedLeft += left;
    if (right != null) onPressedRight += right;
  }
  
  [ContextMenu("fetch")]
  override protected void setup()
  {
    base.setup();
    _arena = ArenaManager.get();
  }

  virtual public void reset()
  {
    
  }

  sealed public override void updateEngine()
  {
    base.updateEngine();

    if (moveTimerDelay > 0f)
    {
      moveTimerDelay -= GameTime.deltaTime;
    }

    if (notInteractiveTimer > 0f)
    {
      notInteractiveTimer -= GameTime.deltaTime;
      return;
    }
    
    //Debug.Log(name + " update " + canUpdate()+" and is visible ? "+isVisible());

    if (isVisible()) updateVisible();
    else updateNotVisible();
  }

  sealed public override void updateEngineLate() { base.updateEngineLate(); }

  virtual protected void updateNotVisible(){}
  virtual protected void updateVisible()
  {
    update_input_keyboard();
  }
  
  protected void update_input_keyboard()
  {
    if(moveTimerDelay <= 0f)
    {
      if (Input.GetKeyUp(KeyCode.UpArrow)) pressed_up();
      if (Input.GetKeyUp(KeyCode.DownArrow)) pressed_down();
      if (Input.GetKeyUp(KeyCode.LeftArrow)) pressed_left();
      if (Input.GetKeyUp(KeyCode.RightArrow)) pressed_right();
    }

    if (Input.GetKeyUp(KeyCode.Escape)) action_back();
  }
  
  virtual protected void pressed_up() { resetTimerDelay(); if (onPressedUp != null) onPressedUp(); }
  virtual protected void pressed_down() { resetTimerDelay(); if (onPressedDown != null) onPressedDown(); }
  virtual protected void pressed_left() { resetTimerDelay(); if (onPressedLeft != null) onPressedLeft(); }
  virtual protected void pressed_right() { resetTimerDelay(); if (onPressedRight != null) onPressedRight(); }

  protected bool isDelaying()
  {
    return moveTimerDelay > 0f;
  }
  protected void resetTimerDelay()
  {
    moveTimerDelay = moveTimerDelayTime;
  }

  virtual protected void action_back() {}

  public Canvas getCanvas()
  {
    return _canvas[0];
  }
  public Canvas getCanvas(string nm)
  {
    for (int i = 0; i < _canvas.Length; i++)
    {
      if (_canvas[i].name.Contains(nm)) return _canvas[i];
    }
    Debug.LogWarning("~ScreenObject~ getCanvas() no canvas named <b>" + nm+"</b>");
    return null;
  }

  public void setCanvasVisibility(string nm, bool flag)
  {
    for (int i = 0; i < _canvas.Length; i++)
    {
      if (_canvas[i].name.Contains(nm))
      {
        //Debug.Log(flag + " for " + nm);
        _canvas[i].enabled = flag;
      }
    }
  }
  
  protected void toggleVisible(bool flag)
  {
    //si le scriptorder fait que le ScreenObject arrive après le Screenmanager ...
    if(_canvas == null) setup();
    
    if (_canvas == null) Debug.LogError("no canvas ? for "+name, gameObject);

    //Debug.Log(name + " visibility ? " + flag+" for "+_canvas.Length+" canvas");
    
    //show all canvas of screen
    for (int i = 0; i < _canvas.Length; i++)
    {
      //Debug.Log(name + "  " + _canvas[i].name);
      if (_canvas[i].enabled != flag)
      {
        //Debug.Log("canvas " + _canvas[i].name + " toggle to " + flag);
        _canvas[i].enabled = flag;
      }
    }

    //Debug.Log(name+" , "+flag, gameObject);
  }
  
  [ContextMenu("show")]
  protected void ctxm_show(){ show(); }

  [ContextMenu("hide")]
  protected void ctxm_hide() { forceHide(); }

  virtual public void show()
  {
    Debug.Log(getStamp()+" show screen : "+name);

    notInteractiveTimer = 0.2f; // to kill interactive frame offset

    transform.position = Vector3.zero;

    toggleVisible(true);

    //Debug.Log(name + " -> show");
  }

  virtual public void hide()
  {
    //Debug.Log("  <color=white>hide()</color> <b>" + name + "</b>");

    if (sticky)
    {
      //Debug.LogWarning("    can't hide " + name + " because is setup as sticky");
      return;
    }

    forceHide();
  }
  
  public void forceHide()
  {
    //Debug.Log("  <color=white>forceHide()</color> <b>" + name + "</b>");

    //dans le cas où y a pas que des canvas
    //ou qu'il y a une seule camera ppale et qu'il faut aligner les choses à 0f
    transform.position = Vector3.down * 3000f;

    toggleVisible(false);

    //Debug.Log(name + " -> forceHide");
  }

  public bool isVisible()
  {
    return mainCanvas.enabled;
    //return transform.position.sqrMagnitude == 0f;
  }

  public void act_button(Button clickedButton)
  {
    process_button_press(clickedButton.name);
  }

  virtual protected void process_button_press(string buttonName)
  { }

  virtual public void act_call_home()
  {
    Debug.Log(getStamp()+" calling <b>home screen</b>");

    ArenaManager am = ArenaManager.get();
    if(am != null)
    {
      am.cancelEndProcess(); // if arena was showing an ending screen, kill this process
      am.arena_cleanup(); 
    }
    
    ScreensManager.open(ScreensManager.ScreenNames.home);
  }

  public override string toString()
  {
    return base.toString() + "\nisVisible ? " + isVisible() + "\ncanvas count ? " + _canvas.Length;
  }
  
  public string extractName()
  {
    string[] split = name.Split('_'); // (screen_xxx)
    return split[1].Substring(0, split[1].Length - 1); // remove ')'
  }

  static public Canvas getCanvas(string screenName, string canvasName) {
    ScreenObject screen = ScreensManager.getScreen(screenName);
    return screen.getCanvas(canvasName);
  }

  static public string getStamp()
  {
    return "<color=white>ScreenObject</color> | ";
  }

}
