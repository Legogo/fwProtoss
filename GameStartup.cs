using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartup : EngineObject {

  public string openingScreen = "home";
  
  protected override void fetchData()
  {
    base.fetchData();
    ScreensManager.get().call("loading");
  }

  public override void onEngineSceneLoaded()
  {
    base.onEngineSceneLoaded();

    ScreensManager.get().call(openingScreen);
  }

}
