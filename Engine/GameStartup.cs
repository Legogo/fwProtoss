using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this script to start a specific screen on loading end
/// It will call the screen "loading" for the loading process
/// </summary>

abstract public class GameStartup : EngineObject {

  public string openingScreen = "";
  
  protected override void build()
  {
    base.build();

    ScreensManager.openByEnum(ScreensManager.ScreenNames.loading);

    setup_preloading();
  }

  protected override void setup()
  {
    base.setup();

    setup_startup();

  }

  abstract protected void setup_preloading();
  abstract protected void setup_startup();
  
  //must be called by child to remove loading screen
  protected void onStartupFinished()
  {
    ScreensManager.closeByName(ScreensManager.ScreenNames.loading.ToString(), true);
  }
}
