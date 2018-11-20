using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ScreensManager {
  
  static protected List<ScreenObject> screens;

  //usual screen names
  public enum ScreenNames {
    home, // home menu
    ingame, // ingame interface (ui)
    pause, // pause screen
    result, // end of round screen, result of round
    loading
  };
  
  static protected void fetchScreens()
  {
    if (screens == null) screens = new List<ScreenObject>();
    screens.Clear();
    screens.AddRange(GameObject.FindObjectsOfType<ScreenObject>());
  }
  
  /// <summary>
  /// returns NON-STICKY visible screen
  /// </summary>
  /// <returns></returns>
  static public ScreenObject getOpenedScreen()
  {
    return screens.Select(x => x).Where(x => !x.sticky && x.isVisible()).FirstOrDefault();
  }

  static public ScreenObject getScreen(ScreenNames nm)
  {
    return getScreen(nm.ToString());
  }
  static public ScreenObject getScreen(string nm)
  {
    fetchScreens();

    ScreenObject so = screens.Select(x => x).Where(x => x.name.EndsWith(nm)).FirstOrDefault();

    if (so != null) return so;
    
    //no warning because it's called before loading to check if screen already exists

    /*
    Debug.LogWarning("~Screens~ getScreen("+nm+") <color=red>no screen that ENDWITH that name</color> (screens count : "+screens.Count+")");
    for (int i = 0; i < screens.Count; i++)
    {
      Debug.Log("  L " + screens[i].name);
    }
    */

    return null;
  }
  
  static public void open(ScreenNames nm) { open(nm.ToString()); }

  /// <summary>
  /// best practice : should never call a screen by name but create a contextual enum
  /// </summary>
  static public ScreenObject open(string nm, string filterName = "")
  {
    Debug.Log("ScreensManager | opening screen of name : <b>" + nm + "</b> , filter ? "+filterName);

    ScreenObject so = getScreen(nm);

    if(so != null)
    {
      changeScreenVisibleState(nm, true, filterName);
      return so;
    }
    
    //si le screen existe pas on essaye de le load
    loadMissingScreen(nm, delegate (ScreenObject loadedScreen)
    {
      Debug.Log("  ... missing screen '"+nm+"' is now loaded, opening");
      changeScreenVisibleState(nm, true, filterName);
    });

    return null;
  }
  
  static protected void changeScreenVisibleState(string scName, bool state, string filter = "", bool force = false)
  {
    fetchScreens();

    //Debug.Log("opening " + scName + " (filter ? " + filter + ")");

    ScreenObject selected = getScreen(scName);
    bool hideOthers = !selected.dontHideOtherOnShow;

    //on opening a specific screen we close all other non sticky screens
    if (hideOthers && state)
    {
      for (int i = 0; i < screens.Count; i++)
      {
        //do nothing with filtered screen
        if (filter.Length > 0 && screens[i].name.EndsWith(filter)) continue;

        if (screens[i] == selected) continue;
        
        screens[i].hide();
      }

    }

    if (state) selected.show();
    else
    {
      if (force) selected.forceHide();
      else selected.hide(); // stickies won't hide
    }

  }

  static public void close(ScreenNames scName, bool force = false) { close(scName.ToString(), force); }

  /// <summary>
  /// </summary>
  /// <param name="nameEnd"></param>
  /// <param name="force">if screen is sticky</param>
  static public void close(string nameEnd, bool force = false, string filter = "")
  {
    changeScreenVisibleState(nameEnd, false, filter, force);
  }

  [ContextMenu("kill all")]
  public void killAll(string filterName = "")
  {
    fetchScreens();

    for (int i = 0; i < screens.Count; i++)
    {
      if (filterName.Length > 0)
      {
        if (screens[i].name.EndsWith(filterName)) continue;
      }

      screens[i].hide();
    }
  }
  
  static protected void loadMissingScreen(string screeName, Action<ScreenObject> onComplete)
  {
    string fullName = screeName;
    if (!fullName.StartsWith("screen-")) fullName = "screen-" + fullName;

    ScreenObject so = getScreen(screeName);

    if(so != null)
    {
      onComplete(so);
      return;
    }
    
    Debug.Log("screen to open : <b>" + fullName + "</b> is not loaded");

    EngineLoader.queryScene(fullName, delegate ()
    {
      so = getScreen(screeName);
      if (so == null) Debug.LogError("ScreensManager ~~ end of screen loading but no ScreenObject of name : <b>"+ screeName+"</b>");
      onComplete(so);
    });
    
  }

  /// <summary>
  /// just display, no state change
  /// </summary>
  /// <param name="state"></param>
  static public void callPauseScreen(bool state)
  {

    if (state) ScreensManager.open(ScreensManager.ScreenNames.pause);
    else ScreensManager.close(ScreensManager.ScreenNames.pause);

  }

}
