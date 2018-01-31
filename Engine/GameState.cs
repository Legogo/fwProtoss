using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : EngineObject {
  
  protected bool _end = false;
  
  public enum eGameStates { LOADING, MENU, LIVE, END };
  static public eGameStates state = eGameStates.LOADING;
  
  public override void updateEngine()
  {
    base.updateEngine();

    GameTime.update();

    if (state != eGameStates.LIVE) return;
    
    //exit app
    if(Input.GetKeyUp(KeyCode.Delete) || Input.GetKeyUp(KeyCode.Backspace))
    {
      Application.Quit();
      return;
    }

    //debug restart
    if(Input.GetKeyUp(KeyCode.R)) {
      Debug.LogWarning("~DEBUG~ debug key press, restart level");
      restartLevel(); // debug restart R key
      return;
    }
    
    //if (dayTimer != null && dayTimer.isTimeout()) callEnd();

  }
  
  /* loop after round end */
  public void restartLevel() {
    
    Debug.Log("<color=green>RESTART LEVEL</color>");
    
    LevelObject[] items = GameObject.FindObjectsOfType<LevelObject>();
    for (int i = 0; i < items.Length; i++)
    {
      items[i].restartLevel();
    }

    for (int i = 0; i < items.Length; i++)
    {
      items[i].restartLevelLate();
    }

    ScreensManager.get().call("ingame");

    state = eGameStates.LIVE;
  }
  
  public void callEnd()
  {
    //Debug.Log("call end");
    state = eGameStates.END;

    SoundManager.stop();
    
  }

  public void callLose()
  {
    Debug.Log("<color=red>lose !</color>");

    SoundManager.stop();

    restartLevel(); // restart on gamestate lose
  }
  
  static protected GameState manager;
  static public GameState get()
  {
    if(manager == null) manager = GameObject.FindObjectOfType<GameState>();
    return manager;
  }
  
}
