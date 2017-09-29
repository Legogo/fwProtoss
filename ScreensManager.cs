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

    EngineManager.get().onLoadingDone += onLoaded;
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

    //some stuff to reinit at each start
    ScreenChallenges.resetSelection();

    //start game
    ScreenObject.call_home();
  }

  public void call(ScreenObject sc)
  {
    call(sc.name);
  }

  public void call(string nm)
  {

    for (int i = 0; i < screens.Length; i++)
    {
      if (screens[i].name.Contains(nm)) screens[i].show();
      else screens[i].hide();
    }

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
