using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

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
      if (screens[i].sticky) continue;
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
  public void open(string nm, string filterName = "")
  {
    Debug.Log("ScreensManager | opening screen of name : <b>" + nm + "</b> , filter ? "+filterName);

    ScreenObject target = getScreen(nm);

    if(target != null)
    {
      openByFilter(nm, filterName);
      return;
    }

    if (target == null) openLoadScreen(nm, delegate(ScreenObject screen)
    {
      openByFilter(nm, filterName);
    });
    
  }

  protected void openByFilter(string nm, string filter = "")
  {
    fetchScreens();

    for (int i = 0; i < screens.Length; i++)
    {
      if (filter.Length > 0)
      {
        if (screens[i].name.Contains(filter)) continue;
      }

      if (screens[i].name.Contains(nm))
      {
        screens[i].show();
      }
      else
      {
        screens[i].hide();
      }
    }

  }

  static public void openByEnum(ScreenNames nm)
  {
    ScreensManager sm = get();
    if (sm == null) Debug.LogWarning("asking to open " + nm.ToString() + " but manager doesn't exist");
    sm.open(nm.ToString());
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
      screens[i].show();
    }

  }

  
  static public void openLoadScreen(string screenNameCont, Action<ScreenObject> onLoaded)
  {
    string fullName = screenNameCont;
    if (!fullName.StartsWith("screen-")) fullName = "screen-" + fullName;

    if (!HalperScene.isSceneOpened(fullName))
    {
      EngineLoader.queryScene(fullName, delegate ()
      {
        ScreenObject so = ScreensManager.get().getScreen(screenNameCont);
        if (so == null) Debug.LogError("end of screen loading but no ScreenObject");
        else
        {
          onLoaded(so);
        }
      });
    }
    else
    {
      ScreenObject so = ScreensManager.get().getScreen(screenNameCont);
      onLoaded(so);
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
