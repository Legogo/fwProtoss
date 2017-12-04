using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreensManager : EngineObject {
  
  protected ScreenObject[] screens;
  
  protected override void build()
  {
    base.build();
    fetchData();
  }
  
  override protected void fetchData()
  {
    base.fetchData();
    screens = GameObject.FindObjectsOfType<ScreenObject>();
  }

  public override void onEngineSceneLoaded()
  {
    base.onEngineSceneLoaded();
    fetchData(); // missing screens

    //Debug.Log("  ScreensManager found " + screens.Length + " screens");
    //for (int i = 0; i < screens.Length; i++) Debug.Log("  -" + screens[i].name);
  }

  public ScreenObject getScreen(string nm)
  {
    for (int i = 0; i < screens.Length; i++)
    {
      if (screens[i].name.Contains(nm)) return screens[i];
    }
    Debug.LogWarning("no screen that CONTAINS name " + nm);
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
  public ScreenObject call(string nm, string filterName = "")
  {
    //Debug.Log("ScreensManager | calling screen : " + nm);

    ScreenObject target = null;

    killAll(nm);

    for (int i = 0; i < screens.Length; i++)
    {
      if(filterName.Length > 0)
      {
        if (screens[i].name.Contains(filterName)) continue;
      }
      
      if (screens[i].name.Contains(nm))
      {
        target = screens[i];
        screens[i].show();
      }
    }

    return target;
  }

  [ContextMenu("kill all")]
  public void killAll(string filterName = "")
  {
    fetchData();

    for (int i = 0; i < screens.Length; i++)
    {
      if (filterName.Length > 0)
      {
        if (screens[i].name.Contains(filterName)) continue;
      }
      
      screens[i].hide();
    }

  }

  [ContextMenu("show all")]
  public void showAll()
  {
    fetchData();

    for (int i = 0; i < screens.Length; i++)
    {
      screens[i].toggleVisible(true);
    }

  }

  static protected ScreensManager manager;
  static public ScreensManager get() { if(manager==null)manager=GameObject.FindObjectOfType<ScreensManager>();return manager; }
  
}
