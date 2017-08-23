using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class ArenaManager : EngineObject {
  
  public float time = 0f;
  
  public enum ArenaState { MENU, LIVE, END }
  protected ArenaState _state;

  protected override void build()
  {
    base.build();

    EngineManager.get().onLoadingDone += restart;
  }

  virtual public void restart()
  {
    Debug.Log("<b>RESTART</b> at "+Time.time);

    freeze = false;
    
    ArenaObject[] aobjs = GameObject.FindObjectsOfType<ArenaObject>();
    for (int i = 0; i < aobjs.Length; i++)
    {
      aobjs[i].restart();
    }

    _state = ArenaState.LIVE;
  }

  protected override void update()
  {
    base.update();
    
    if (Input.GetKeyUp(KeyCode.Space))
    {
      restart();
    }

    //speed up debug
    float mul = 1f;

    if (Input.GetKey(KeyCode.P))
    {
      mul = 100f;
    }

    time += Time.deltaTime * mul;
  }

  public void event_end()
  {
    _state = ArenaState.END;

    //send info to all arena objects
    ArenaObject[] aobjs = GameObject.FindObjectsOfType<ArenaObject>();
    for (int i = 0; i < aobjs.Length; i++)
    {
      aobjs[i].event_end();
    }

    StartCoroutine(processEnd());
  }
  
  virtual protected IEnumerator processEnd()
  {
    yield return null;
  }

  public float getElapsedTime()
  {
    return time;
  }

  public ArenaState getState() { return _state; }
  public bool isAtState(ArenaState st) { return _state == st; }
  public bool isLive() { return isAtState(ArenaState.LIVE); }
  public bool isEnd() { return isAtState(ArenaState.END); }

  static protected ArenaManager _manager;
  static public ArenaManager get()
  {
    if (_manager == null) _manager = GameObject.FindObjectOfType<ArenaManager>();
    return _manager;
  }
}
