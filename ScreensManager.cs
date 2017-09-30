using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreensManager : EngineObject {
  
  protected ScreenObject[] screens;

  protected override void build()
  {
    base.build();

    fetchData();

    call("loading");

    EngineManager em = EngineManager.get();
    if(em != null)
    {
      em.onLoadingDone += onLoaded;
    }

  }
  
  override protected void fetchData()
  {
    base.fetchData();
    screens = GameObject.FindObjectsOfType<ScreenObject>();
  }

  protected ScreenObject getScreen(string nm)
  {
    for (int i = 0; i < screens.Length; i++)
    {
      if (screens[i].name.Contains(nm)) return screens[i];
    }
    return null;
  }

  protected void onLoaded()
  {
    //###
    for (int i = 0; i < screens.Length; i++)
    {
      screens[i].reset();
    }
    
  }

  /* return only the first found */
  public ScreenObject call(string nm)
  {
    //Debug.Log("ui calling " + nm);
    ScreenObject target = null;

    for (int i = 0; i < screens.Length; i++)
    {
      if (screens[i].name.Contains(nm))
      {
        target = screens[i];
        screens[i].show();
      }
      else screens[i].hide();
    }
    return target;
  }

  [ContextMenu("kill all")]
  public void killAll()
  {
    fetchData();

    call("none");
  }

  [ContextMenu("show all")]
  public void showAll()
  {
    fetchData();

    for (int i = 0; i < screens.Length; i++)
    {
      screens[i].showAll();
    }

  }

  static public ScreensManager get() { return GameObject.FindObjectOfType<ScreensManager>(); }
}
