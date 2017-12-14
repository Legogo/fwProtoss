using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartup : EngineObject {

  public string openingScreen = "home";
  
  protected override void fetchData()
  {
    base.fetchData();

    //show loading
    ScreensManager.get().call("loading");
  }

  public override void onEngineSceneLoaded()
  {
    base.onEngineSceneLoaded();

    StartCoroutine(processWaitStartup());
    
  }

  IEnumerator processWaitStartup()
  {
    //on doit attendre au moins 1 frame pour que les autres onEngineSceneLoaded aient fini
    yield return null;
    
    //fake wait
    yield return new WaitForSeconds(0.5f);

    Debug.Log("starting game by opening screen <b>" + openingScreen+"</b>");

    ScreensManager.get().call(openingScreen);
  }

}
