using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this script to start a specific screen on loading end
/// It will call the screen "loading" for the loading process
/// </summary>

public class GameStartup : EngineObject {

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

#if UNITY_EDITOR
    if (HiddenGameManager.get().settings.skip_home_menu)
    {
      Debug.Log("skip home menu");

      ScreenObject activeScreen = ScreensManager.get().getOpenedScreen();
      Debug.Log(activeScreen.name);

      if(activeScreen as ScreenMenuHome)
      {
        Debug.LogWarning("~DEBUG~ <color=orange><b>skipping home screen</b></color> " + name, gameObject);
        (activeScreen as ScreenMenuHome).skip();
      }
      
    }
#endif
    
  }
  
}
