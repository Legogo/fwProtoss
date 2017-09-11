using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class ArenaManager : EngineObject {
  
  public float time = 0f; // elasped time

  protected float liveFreezeTimer = 0f;

  public enum ArenaState { MENU, LIVE, END }
  protected ArenaState _state;

  protected Coroutine coProcessEnd;

  public List<ArenaObject> arenaObjects = new List<ArenaObject>();

  protected override void build()
  {
    base.build();

    //EngineManager.get().onLoadingDone += restart;
  }

  virtual public void restart()
  {
    Debug.Log("<b>RESTART</b> at "+Time.time);

    _freeze = false;

    for (int i = 0; i < arenaObjects.Count; i++)
    {
      arenaObjects[i].restart();
    }

    _state = ArenaState.LIVE;

    liveFreezeTimer = 1f;
  }

  public override void update()
  {
    base.update();

    if (!EngineManager.isLive()) return;

    if (Input.GetKeyUp(KeyCode.Space))
    {
      restart();
      return;
    }

    //speed up debug arena timer
    float mul = 1f;
    if (Input.GetKey(KeyCode.P)) mul = 100f;
    time += Time.deltaTime * mul;
    
    //update all aobs
    for (int i = 0; i < arenaObjects.Count; i++)
    {
      arenaObjects[i].updateArena();
    }

    if (liveFreezeTimer > 0f)
    {
      liveFreezeTimer -= Time.deltaTime;
    }

  }

  virtual public void kill()
  {
    if(coProcessEnd != null)
    {
      StopCoroutine(coProcessEnd);
    }
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

    coProcessEnd = StartCoroutine(processEnd());
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
  public bool isLive() { return liveFreezeTimer < 0f && isAtState(ArenaState.LIVE); }
  public bool isEnd() { return isAtState(ArenaState.END); }

  static protected ArenaManager _manager;
  static public ArenaManager get()
  {
    if (_manager == null) _manager = GameObject.FindObjectOfType<ArenaManager>();
    return _manager;
  }
}
