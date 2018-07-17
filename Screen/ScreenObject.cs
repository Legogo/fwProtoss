using System;
using UnityEngine;

public class ScreenObject : EngineObject
{
  protected ArenaManager _arena;

  public bool sticky = false; // can't be hidden
  
  protected Canvas[] _canvas;

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

    //if (_canvas == null) Debug.LogError("wat ?");

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
    
    //Debug.Log(name + " update " + canUpdate());

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
  virtual public void show()
  {
    //Debug.Log("~Screen~ <color=green>show()</color> <b>"+name + "</b>");

    //if(ScreensManager.get() != null) ScreensManager.get().killAll();

    notInteractiveTimer = 0.2f; // to kill interactive frame offset

    transform.position = Vector3.zero;

    toggleVisible(true);
  }

  virtual public void hide()
  {
    if (sticky) return;

    //Debug.Log("~Screen~ <color=white>hide()</color> <b>" + name + "</b>");

    forceHide();
  }

  [ContextMenu("hide")]
  public void forceHide()
  {
    transform.position = Vector3.down * 3000f;

    toggleVisible(false);
  }

  public bool isVisible()
  {
    return transform.position.sqrMagnitude == 0f;
  }

  virtual public void act_call_home()
  {
    ScreensManager.openByEnum(ScreensManager.ScreenNames.home);
  }

  public override string toString()
  {
    return base.toString() + "\nisVisible ? " + isVisible() + "\ncanvas count ? " + _canvas.Length;
  }
  
  static public Canvas getCanvas(string screenName, string canvasName) {
    ScreenObject screen = ScreensManager.get().getScreen(screenName);
    return screen.getCanvas(canvasName);
  }
}
