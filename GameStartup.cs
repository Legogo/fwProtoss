using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this script to start a specific screen on loading end
/// It will call the screen "loading" for the loading process
/// </summary>

abstract public class GameStartup : EngineObject {

  public string openingScreen;

  protected float timer = 0f;
  
  /* this is called after eveything is done reacting to loading end */
  protected override void setup()
  {
    base.setup();
    
    StopAllCoroutines();
    StartCoroutine(processStartup());
  }
  
  IEnumerator processStartup()
  {

    setup_preloading();

    //Debug.Log(timer);

    while (timer > 0f)
    {
      timer -= Time.deltaTime;
      yield return null;
    }

    yield return null;

    Debug.Log("~Startup~ starting ... opening screen : <b>" + openingScreen + "</b>");

    ScreensManager.get().call(openingScreen);

    yield return null;

    ScreenLoading sl = ScreenLoading.get();
    if (sl != null) {
      sl.hideLoadingScreen();
    }

    yield return null;

    setup_startup();
  }

  abstract protected void setup_preloading();
  abstract protected void setup_startup();
}
