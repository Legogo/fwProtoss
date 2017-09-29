using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenObject : EngineObject
{
  public bool sticky = false;
  protected Canvas[] _canvas;

  protected override void build()
  {
    base.build();

    fetchData();

    if (_canvas == null) Debug.LogError("no canvas ?");
    
    //if (_canvas == null) Debug.LogError("wat ?");
  }

  public override void onEngineSceneLoaded()
  {
    base.onEngineSceneLoaded();
    
    hide();
  }

  [ContextMenu("fetch")]
  override protected void fetchData()
  {
    base.fetchData();
    _canvas = transform.GetComponentsInChildren<Canvas>();
  }

  protected void toggleVisible(bool flag)
  {
    if(!Application.isPlaying) fetchData();

    if (sticky) flag = true;

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
    //ScreensManager.get().call("");
    if(ScreensManager.get() != null) ScreensManager.get().killAll();

    toggleVisible(true);
  }

  public void showAll()
  {
    toggleVisible(true);
  }
  
  [ContextMenu("hide")]
  virtual public void hide()
  {
    if (sticky) return;
    toggleVisible(false);
  }

  public bool isVisible()
  {
    return _canvas[0].enabled;
  }

  public void act_call_home()
  {
    call_home();
  }

  static public void call_home()
  {
    
    ScreensManager sm = ScreensManager.get();
    if(sm != null)
    {
      sm.call("home");
    }
    
  }

  public void act_ingame_retry()
  {
    ArenaSnakeManager.get().restart();
  }

}
