using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this script to start a specific screen on loading end
/// It will call the screen "loading" for the loading process
/// </summary>

public class GameStartup : EngineObject {

  public string openingScreen = "home";
  
  protected override void fetchData()
  {
    base.fetchData();

    //need to be after loading (because engine manager might be in a resource scene)
    EngineManager em = EngineManager.get();
    if (em == null) Debug.LogError("no engine manager ?");
    else em.onLoadingDone += engineLoadingDone;

    //show loading
    ScreensManager.get().call("loading");
  }
  
  /* this is called after eveything is done reacting to loading end */
  protected void engineLoadingDone()
  {
    Debug.Log("~Startup~ starting ...");
    ScreensManager.get().call(openingScreen);
  }
  
}
