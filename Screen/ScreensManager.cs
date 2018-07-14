using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreensManager : EngineObject {
  
  protected ScreenObject[] screens;
  
  protected void fetchScreens()
  {
    screens = GameObject.FindObjectsOfType<ScreenObject>();
  }
  
  public ScreenObject getOpenedScreen()
  {
    for (int i = 0; i < screens.Length; i++)
    {
      if (screens[i].isVisible()) return screens[i];
    }
    return null;
  }

  public ScreenObject getScreen(string nm)
  {
    fetchScreens();

    for (int i = 0; i < screens.Length; i++)
    {
      if (screens[i].name.Contains(nm)) return screens[i];
    }
    //Debug.LogWarning("~Screens~ getScreen("+nm+") no screen that CONTAINS this name");
    return null;
  }
  
  /* return only the first found */
  public ScreenObject call(string nm, string filterName = "")
  {
    Debug.Log("ScreensManager | opening ScrenObject : <b>" + nm + "</b> , filter ? "+filterName);

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
    fetchScreens();

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
    fetchScreens();

    for (int i = 0; i < screens.Length; i++)
    {
      screens[i].toggleVisible(true);
    }

  }

  static protected ScreensManager manager;
  static public ScreensManager get() { 
    if(manager == null)manager=GameObject.FindObjectOfType<ScreensManager>();
    if(manager == null) {
      GameObject obj = new GameObject("(screens)");
      manager = obj.AddComponent<ScreensManager>();
    }
    return manager;
  }
  
}
