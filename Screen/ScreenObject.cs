﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenObject : EngineObject
{
  protected ArenaManager _arena;
  public bool sticky = false;
  
  protected Canvas[] _canvas;

  protected override void build()
  {
    base.build();

    _canvas = transform.GetComponentsInChildren<Canvas>();
    if (_canvas == null) Debug.LogError("no canvas ?");

    //if (_canvas == null) Debug.LogError("wat ?");

    hide();
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
    if (isVisible()) updateVisible();
  }

  virtual protected void updateVisible()
  {
    update_input_keyboard();
  }
  
  protected void update_input_keyboard()
  {
    if (Input.GetKeyUp(KeyCode.UpArrow)) keyboard_up();
    if (Input.GetKeyUp(KeyCode.DownArrow)) keyboard_down();
    if (Input.GetKeyUp(KeyCode.Escape)) keyboard_esc();
  }

  virtual protected void keyboard_esc() {
    if (isVisible() && !sticky)
    {
      action_back();
    }
  }
  virtual protected void keyboard_up() { }
  virtual protected void keyboard_down() { }

  virtual protected void action_back() {

    call_home();
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

  public void toggleVisible(bool flag)
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
    Debug.Log("~Screen~ <color=green>show()</color> <b>"+name + "</b>");

    //if(ScreensManager.get() != null) ScreensManager.get().killAll();

    transform.position = Vector3.zero;

    toggleVisible(true);
  }

  virtual public void hide()
  {
    if (sticky) return;

    Debug.Log("~Screen~ <color=red>hide()</color> <b>" + name + "</b>");

    forceHide();
  }

  [ContextMenu("hide")]
  public void forceHide() {
    transform.position = Vector3.down * 3000f;

    toggleVisible(false);
  }

  public override bool canUpdate()
  {
    if (!isVisible()) return false;
    return base.canUpdate();
  }
  
  public bool isVisible()
  {
    return transform.position.sqrMagnitude == 0f;
  }

  virtual public void act_call_home()
  {
    call_home();
  }

  public override string toString()
  {
    return base.toString() + "\nisVisible ? " + isVisible() + "\ncanvas count ? " + _canvas.Length;
  }

  static public void call_home()
  {
    ScreensManager.openByEnum(ScreensManager.ScreenNames.home);
  }

  static public Canvas getCanvas(string screenName, string canvasName) {
    ScreenObject screen = ScreensManager.get().getScreen(screenName);
    return screen.getCanvas(canvasName);
  }
}
