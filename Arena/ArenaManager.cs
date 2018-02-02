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

  protected Canvas pauseCanvas;

  public List<ArenaObject> arenaObjects = new List<ArenaObject>();
  
  virtual public void restart_normal() {
    time = 0f;
    restart_setup();
  }

  virtual public void restart_setup() {
    _unfreeze = false;
    _state = ArenaState.LIVE;
  }

  /* how the arena can get the pause screen */
  virtual protected void fetchPauseCanvasScreen()
  {
    if(ScreensManager.get() != null)
    {
      ScreenObject obj = ScreensManager.get().getScreen("ingame");
      if (obj != null) pauseCanvas = obj.getCanvas("pause");
    }
  }

  protected override void fetchGlobal()
  {
    base.fetchGlobal();
    fetchPauseCanvasScreen();
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
    }
    
  }
  
  protected void update_time()
  {

    //speed up debug arena timer
    float mul = 1f;
    
    if (Input.GetKey(KeyCode.S)) mul = 100f; //debug, make ingame time go faster
    time += Time.deltaTime * mul;

    if (liveFreezeTimer > 0f)
    {
      liveFreezeTimer -= Time.deltaTime;
    }
    
  }

  virtual protected void update_round()
  {
    //update all aobs
    //Debug.Log("update_round (" + arenaObjects.Count + ")");
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
    string ct = "live freeze timer ? " + liveFreezeTimer;
    ct += "\nfreeze ? " + isFreezed();
    ct += "\nstate ? " + _state;
    return ct;
  }

  public ArenaState getState() { return _state; }
  protected bool isAtState(ArenaState st) { return _state == st; }
  public bool isArenaStateLive() {

    //pause modale kill gameplay
    if (pauseCanvas != null && pauseCanvas.enabled) return false;

    //Debug.Log(_state);

    return liveFreezeTimer <= 0f && isAtState(ArenaState.LIVE);
  }
  public bool isArenaStateEnd() { return isAtState(ArenaState.END); }

  static protected ArenaManager _manager;
  static public ArenaManager get()
  {
    if (_manager == null) _manager = GameObject.FindObjectOfType<ArenaManager>();
    return _manager;
  }
}
