using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this script to start a specific screen on loading end
/// It will call the screen "loading" for the loading process
/// </summary>

abstract public class GameStartup : EngineObject {

  public string openingScreen;
  
  /* this is called after eveything is done reacting to loading end */
  protected override void setup()
  {
    base.setup();
    Debug.Log("~Startup~ starting ... opening screen : <b>" + openingScreen + "</b>");

    StopAllCoroutines();
    StartCoroutine(processStartup());
  }

  IEnumerator processStartup()
  {
    
    ScreensManager.get().call(openingScreen);

    yield return null;

    setup_startup();
  }

  abstract protected void setup_startup();
}
