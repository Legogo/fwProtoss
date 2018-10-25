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
    result, // end of round screen, result of round
    loading
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
  public void open(string nm, string filterName = "")
  {
    Debug.Log("ScreensManager | opening screen of name : <b>" + nm + "</b> , filter ? "+filterName);

    ScreenObject so = getScreen(nm);
    if(so == null)
    {
      loadMissingScreen(nm, delegate (ScreenObject loadedScreen)
      {
        Debug.Log("  ... missing screen is now loaded, opening");
        openByFilter(nm, filterName);
      });
      return;
    }

    openByFilter(nm, filterName);
  }

  /// <summary>
  /// this will open a screen and close other non-sticky screens
  /// </summary>
  /// <param name="nm"></param>
  /// <param name="filter"></param>
  protected void openByFilter(string nm, string filter = "")
  {
    fetchScreens();

    //Debug.Log("opening " + nm+" (filter ? "+filter+")");

    for (int i = 0; i < screens.Length; i++)
    {
      if (filter.Length > 0)
      {
        if (screens[i].name.Contains(filter)) continue;
      }

      //Debug.Log("  L " +screens[i].name);
      if (screens[i].name.Contains(nm))
      {
        screens[i].show();
      }
      else
      {
        screens[i].hide(); // stickies won't hide
      }
    }

  }

  public void close(string nameEnd, bool force)
  {
    fetchScreens();

    //Debug.Log("closing " + nameEnd);

    for (int i = 0; i < screens.Length; i++)
    {
      //Debug.Log("  L " + screens[i].name);

      if (screens[i].name.EndsWith(nameEnd))
      {
        if (force) screens[i].forceHide();
        else screens[i].hide();
      }
    }
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

  
  protected void loadMissingScreen(string screenNameCont, Action<ScreenObject> onComplete)
  {
    string fullName = screenNameCont;
    if (!fullName.StartsWith("screen-")) fullName = "screen-" + fullName;

    ScreenObject so = ScreensManager.get().getScreen(screenNameCont);

    if(so != null)
    {
      onComplete(so);
      return;
    }
    
    Debug.Log("screen to open : <b>" + fullName + "</b> is not loaded");

    EngineLoader.queryScene(fullName, delegate ()
    {
      so = ScreensManager.get().getScreen(screenNameCont);
      if (so == null) Debug.LogError("end of screen loading but no ScreenObject");
      onComplete(so);
    });
    
  }

  static public void closeByName(string nm, bool force)
  {
    ScreensManager sm = get();
    sm.close(nm, force);
  }

  static public void openByName(string nm)
  {
    ScreensManager sm = get();
    if (sm == null) Debug.LogWarning("asking to open " + nm.ToString() + " but manager doesn't exist");
    sm.open(nm.ToString());
  }
  static public void openByEnum(ScreenNames nm)
  {
    openByName(nm.ToString());
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
