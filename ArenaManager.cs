using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class ArenaManager : EngineObject {
  
  public float time = 0f; // elasped time

  protected float liveFreezeTimer = 0f;

  public enum ArenaState { IDLE, MENU, LIVE, END }
  protected ArenaState _state = ArenaState.IDLE;

  protected Coroutine coProcessEnd;

  public List<ArenaObject> arenaObjects = new List<ArenaObject>();

  protected override void build()
  {
    base.build();

    //EngineManager.get().onLoadingDone += restart;
  }
  
  /* game */
  virtual public void restart_round()
  {
    //Debug.Log("<b>RESTARTING</b> ARENA ROUND at "+Time.time);

    time = 0f;

    _freeze = false;
    
    for (int i = 0; i < arenaObjects.Count; i++)
    {
      arenaObjects[i].restart();
    }

    _state = ArenaState.LIVE;

    liveFreezeTimer = 1f;
  }

  /* background */
  virtual public void restart_menu()
  {
    //Debug.Log("<b>RESTARTING</b> ARENA MENU at " + Time.time);

    time = 0f;

    _freeze = false;

    _state = ArenaState.MENU;
  }

  public override void updateEngine()
  {
    base.updateEngine();

    //freeze timer at start NEED to update
    update_time();

    if (_state == ArenaState.MENU)
    {
      update_menu();
    }
    else if(_state == ArenaState.LIVE || _state == ArenaState.END)
    {
      update_round();
      update_debug();
    }
    
  }

  protected void update_debug()
  {

#if UNITY_EDITOR
    // ==== DEBUG KEYS

    if (Input.GetKeyUp(KeyCode.Backspace))
    {
      restart_round();
    }
    else if (Input.GetKeyUp(KeyCode.Escape))
    {
      Debug.LogWarning("DEBUG | stopped session");
      debug_round_cancel();
    }

#endif
  }

  protected void update_time()
  {

    //speed up debug arena timer
    float mul = 1f;
    
    //debug, make ingame time go faster
    if (Input.GetKey(KeyCode.P)) mul = 100f;
    time += Time.deltaTime * mul;

    if (liveFreezeTimer > 0f)
    {
      liveFreezeTimer -= Time.deltaTime;
    }
    
  }

  virtual protected void update_round()
  {

    //update all aobs
    //Debug.Log("ARENA update (" + arenaObjects.Count + ")");
    for (int i = 0; i < arenaObjects.Count; i++)
    {
      arenaObjects[i].updateArena();
    }

  }

  virtual protected void update_menu()
  {
    for (int i = 0; i < arenaObjects.Count; i++)
    {
      arenaObjects[i].updateMenu();
    }

  }

  virtual public void debug_round_cancel() {
    event_end();
  }

  virtual public void kill()
  {
    if(coProcessEnd != null)
    {
      StopCoroutine(coProcessEnd);
    }

    //for (int i = 0; i < arenaObjects.Count; i++) arenaObjects[i].kill();

    UiAnimation.killAll();
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

    if (coProcessEnd != null)
    {
      Debug.LogError("already processing end ??");
      return;
    }
    
    coProcessEnd = StartCoroutine(processEnd());
  }
  
  virtual protected IEnumerator processEnd()
  {
    yield return null;

    coProcessEnd = null;
  }

  public float getElapsedTime()
  {
    return time;
  }

  override public string toString()
  {
    string ct = name + " | live freeze timer ? " + liveFreezeTimer;
    ct += "\n  freeze ? " + isFreezed() + " , state ? " + _state;
    return ct;
  }

  public ArenaState getState() { return _state; }
  protected bool isAtState(ArenaState st) { return _state == st; }
  public bool isArenaStateLive() { return liveFreezeTimer < 0f && isAtState(ArenaState.LIVE); }
  public bool isArenaStateEnd() { return isAtState(ArenaState.END); }

  static protected ArenaManager _manager;
  static public ArenaManager get()
  {
    if (_manager == null) _manager = GameObject.FindObjectOfType<ArenaManager>();
    return _manager;
  }
}
