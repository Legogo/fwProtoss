using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreensManager : EngineObject {
  
  protected ScreenObject[] screens;

  //usual screen names
  public enum ScreenNames {
    home, // home menu
    ingame, // ingame interface (ui)
    pause, // pause screen
    result // end of round screen, result of round
  };

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

  public ScreenObject getScreen(ScreenNames nm)
  {
    return getScreen(nm.ToString());
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
  
  /// <summary>
  /// best practice : should never call a screen by name but create a contextual enum
  /// </summary>
  /// <returns>first screen found</returns>
  public ScreenObject call(string nm, string filterName = "")
  {
    Debug.Log("ScreensManager | opening screen of name : <b>" + nm + "</b> , filter ? "+filterName);

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

  static public ScreenObject openByEnum(ScreenNames nm)
  {
    ScreensManager sm = get();
    if (sm == null) { Debug.LogWarning("asking to open " + nm.ToString() + " but manager doesn't exist"); return null; }
    return sm.call(nm.ToString());
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
