using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenObject : EngineObject
{
  public bool sticky = false;
  
  protected Canvas[] _canvas;

  protected override void build()
  {
    base.build();

    fetchGlobal(); //specific case

    if (_canvas == null) Debug.LogError("no canvas ?");

    //if (_canvas == null) Debug.LogError("wat ?");

    hide();
  }
  
  [ContextMenu("fetch")]
  override protected void fetchGlobal()
  {
    base.fetchGlobal();

    _canvas = transform.GetComponentsInChildren<Canvas>();

    //Debug.Log(name + " --> " + _canvas);
  }

  virtual public void reset()
  {

  }

  public override void updateEngineLate()
  {
    base.updateEngineLate();

    if(isVisible() && !sticky) {
      if (Input.GetKeyUp(KeyCode.Escape))
      {
        //Debug.Log(name + " pressed back button");
        action_back();
      }
    }
    
  }
  
  virtual protected void action_back() {

    call_home();
  }

  public Canvas getCanvas(string nm)
  {
    for (int i = 0; i < _canvas.Length; i++)
    {
      if (_canvas[i].name.Contains(nm)) return _canvas[i];
    }
    Debug.LogWarning("no canvas that CONTAINS " + nm);
    return null;
  }

  public void setCanvasVisibility(string nm, bool flag)
  {
    for (int i = 0; i < _canvas.Length; i++)
    {
      if (_canvas[i].name.Contains(nm)) _canvas[i].enabled = flag;
    }
  }

  public void toggleVisible(bool flag)
  {
    //si le scriptorder fait que le ScreenObject arrive après le Screenmanager ...
    if(_canvas == null) fetchGlobal();
    
    if (_canvas == null) Debug.LogError("no canvas ? for "+name, gameObject);

    //show all canvas of screen
    for (int i = 0; i < _canvas.Length; i++)
    {
      //Debug.Log(name + "  " + _canvas[i].name);
      if (_canvas[i].enabled != flag) _canvas[i].enabled = flag;
    }

    //Debug.Log(name+" , "+flag, gameObject);
  }
  
  [ContextMenu("show")]
  virtual public void show()
  {
    Debug.Log("~Screen~ show() <b>"+name + "</b>");

    //if(ScreensManager.get() != null) ScreensManager.get().killAll();

    transform.position = Vector3.zero;

    toggleVisible(true);
  }

  virtual public void hide()
  {
    if (sticky) return;

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
    //at least one
    /*
    for (int i = 0; i < _canvas.Length; i++)
    {
      if (_canvas[i].enabled) return true;
    }
    return false;
    */

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

    ScreensManager sm = ScreensManager.get();
    if(sm != null)
    {
      sm.call("home");
    }
    
  }

  static public Canvas getCanvas(string screenName, string canvasName) {
    ScreenObject screen = ScreensManager.get().getScreen(screenName);
    return screen.getCanvas(canvasName);
  }
}
