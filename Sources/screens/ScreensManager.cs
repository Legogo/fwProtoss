using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using scaffolder.pocEngine;

public class ScreensManager {
  
  static protected List<ScreenObject> screens;

  public enum ScreenType
  {
    menu,
    ingame
  }

  //usual screen names
  public enum ScreenNames
  {
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
  
  static public bool hasOpenScreenOfType(ScreenType type)
  {
    ScreenObject so = getOpenedScreen();
    if (so == null) return false;
    return so.type == type;
  }

  /// <summary>
  /// returns NON-STICKY visible screen
  /// </summary>
  /// <returns></returns>
  static public ScreenObject getOpenedScreen()
  {
    if (screens == null) return null;
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
  
  static public void unloadScreen(string nm)
  {
    ScreenObject so = getScreen(nm);
    if (so != null)
    {
      Debug.Log("unloading screen | asked name : " + nm);
      so.unload();
    }
  }

  static public ScreenObject open(ScreenNames nm, string filter = "") { return open(nm.ToString(), filter); }
  static public ScreenObject open(string nm, Action onComplete) { return open(nm, "", onComplete); }

  /// <summary>
  /// best practice : should never call a screen by name but create a contextual enum
  /// this function won't return a screen that is not already loaded
  /// </summary>
  static public ScreenObject open(string nm, string filterName = "", Action onComplete = null)
  {
    Debug.Log("ScreensManager | opening screen of name : <b>" + nm + "</b> , filter ? " + filterName);

    // -- removing startup "screen-" prefix
    string prefix = "screen-";
    if (nm.StartsWith(prefix))
    {
      nm = nm.Substring(prefix.Length, nm.Length - prefix.Length);
    }

    ScreenObject so = getScreen(nm);

    if(so != null)
    {
      changeScreenVisibleState(nm, true, filterName);
      return so;
    }
    
    //si le screen existe pas on essaye de le load
    loadMissingScreen(nm, delegate (ScreenObject loadedScreen)
    {
      Debug.Log("  ... missing screen '" + nm + "' is now loaded, opening");
      changeScreenVisibleState(nm, true, filterName);
      if (onComplete != null) onComplete();
    });

    return null;
  }
  
  static protected void changeScreenVisibleState(string scName, bool state, string containsFilter = "", bool force = false)
  {
    fetchScreens();

    //Debug.Log("opening " + scName + " (filter ? " + filter + ")");

    ScreenObject selected = getScreen(scName);
    if(selected == null)
    {
      Debug.LogWarning("trying to change visibility of screen "+scName+" but this ScreenObject doesn't exist");
      return;
    }

    bool hideOthers = !selected.dontHideOtherOnShow;

    //Debug.Log(selected.name + " visibilty to " + state+" (filter ? "+containsFilter+" | dont hide other ? "+selected.dontHideOtherOnShow+" => hide others ? "+hideOthers+")");

    //on opening a specific screen we close all other non sticky screens
    if (hideOthers && state)
    {
      for (int i = 0; i < screens.Count; i++)
      {
        if (screens[i] == selected) continue;

        //do nothing with filtered screen
        if (containsFilter.Length > 0 && screens[i].name.Contains(containsFilter)) continue;
        
        screens[i].hide();
        //Debug.Log("  L "+screens[i].name + " hidden");
      }

    }
    
    if (state) selected.show();
    else
    {
      if (force) selected.forceHide();
      else selected.hide(); // stickies won't hide
    }

  }

  static public void close(ScreenNames scName) { close(scName.ToString()); }
  static public void close(string scName) { close(scName, "", false); }
  static public void close(ScreenNames scName, bool force = false) { close(scName.ToString(), "", force); }
  static public void close(ScreenNames scName, string filter = "", bool force = false) { close(scName.ToString(), filter, force); }

  /// <summary>
  /// </summary>
  /// <param name="nameEnd"></param>
  /// <param name="force">if screen is sticky</param>
  static protected void close(string nameEnd, string filter = "", bool force = false)
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
  
  static protected void loadMissingScreen(string screenName, Action<ScreenObject> onComplete)
  {
    ScreenLoading.showLoadingScreen();

    //re-add "screen-" prefix if missing
    string fullName = screenName;
    if (!fullName.StartsWith("screen-")) fullName = "screen-" + fullName;

    // first search if already exists
    ScreenObject so = getScreen(fullName);
    if (so != null)
    {
      onComplete(so);
      return;
    }
    
    Debug.Log("screen to open : <b>" + fullName + "</b> is not loaded");

    EngineLoader.queryScene(fullName, delegate ()
    {
      so = getScreen(screenName);
      if (so == null)
      {
        Debug.LogError("ScreensManager | end of screen loading (name given : " + screenName + ") but no <ScreenObject> returned");
      }
      onComplete(so);
    });
    
  }

  /// <summary>
  /// just display, no state change
  /// </summary>
  /// <param name="state"></param>
  static public void callPauseScreen(bool state, string filter = "")
  {

    if (state) ScreensManager.open(ScreensManager.ScreenNames.pause, filter);
    else ScreensManager.close(ScreensManager.ScreenNames.pause, filter);

  }

}
