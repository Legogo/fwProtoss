using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// onLoadingFinished()
/// restart_normal()
/// 
/// update_round()
/// while(checkRoundEnd()) ...
///   
/// event_round_end()
/// process_before_restart() coroutine
/// 
/// </summary>

abstract public class ArenaManager : EngineObject {
  
  public float round_time = 0f; // round elasped time

  protected float liveFreezeTimer = 0f;

  public enum ArenaState { IDLE, MENU, LIVE, END }
  protected ArenaState _state = ArenaState.IDLE;

  protected Coroutine coProcessEnd;

  protected Canvas pauseCanvas;
  
  public List<ArenaObject> arenaObjects = new List<ArenaObject>();

  protected override void build()
  {
    base.build();
    EngineManager.get().onLoadingDone += onLoadingFinished;

    EngineEventSystem.onPause += onPause;
  }

  virtual protected void onPause(bool state)
  {
    Debug.Log("pause ! " + state);
    setFreeze(state);
  }

  /* this is default behavior for arena manager, it needs to be overrided if need to delay startup of arena */
  virtual protected void onLoadingFinished()
  {
    Debug.Log("~ArenaManager~ loading is finished, calling restart_normal() <- this should be overriten by context");
    restart_normal();
  }

  protected override void setup()
  {
    base.setup();
    fetchPauseCanvasScreen();
  }

  /* how the arena can get the pause screen */
  virtual protected void fetchPauseCanvasScreen()
  {
    if (ScreensManager.get() != null)
    {
      ScreenObject obj = ScreensManager.get().getScreen("ingame");
      if (obj != null) pauseCanvas = obj.getCanvas("pause");
    }
  }


  /// <summary>
  /// normal path to restart a round
  /// </summary>
  virtual public void restart_normal() {
    Debug.Log("~<b>Arena</b>~ restart_normal");
    round_time = 0f;
    restart_setup();
  }

  /// <summary>
  /// describe how to setup a restart
  /// </summary>
  virtual protected void restart_setup() {

    for (int i = 0; i < arenaObjects.Count; i++)
    {
      arenaObjects[i].arena_round_restart();
    }
    
    setFreeze(false);

    //Debug.Log("live !");
    _state = ArenaState.LIVE;
  }

  public override void updateEngine()
  {
    base.updateEngine();
    
    //freeze timer at start NEED to update
    update_time();

    //Debug.Log("update arena " + _state);

    //Debug.Log(GetType() + " , update engine");

    if (_state == ArenaState.MENU)
    {
      update_menu();
    }
    else if(_state == ArenaState.LIVE)
    {
      update_round();
      update_round_late();

      if(checkRoundEnd())
      {
        event_round_end();
      }
    }
    else if(_state == ArenaState.END)
    {
      update_round_end();
    }
    
  }

  abstract protected bool checkRoundEnd();

  virtual protected void update_menu(){}
  
  protected void update_time()
  {
    round_time += GameTime.deltaTime;

    if (liveFreezeTimer > 0f)
    {
      liveFreezeTimer -= GameTime.deltaTime;
    }
    
  }

  virtual protected void update_round()
  {
    //update all aobs
    //Debug.Log("update_round (" + arenaObjects.Count + ")");
    for (int i = 0; i < arenaObjects.Count; i++)
    {
      if (arenaObjects[i].isFreezed()) continue;
      arenaObjects[i].updateArena();
    }

  }

  virtual protected void update_round_late()
  {
    for (int i = 0; i < arenaObjects.Count; i++)
    {
      if (arenaObjects[i].isFreezed()) continue;
      arenaObjects[i].updateArenaLate();
    }
  }

  virtual protected void update_round_end()
  {

  }
  
  /// <summary>
  /// called if system need to interrupt gameplay
  /// </summary>
  virtual public void round_stop()
  {
    setArenaIdle();
    
    //send info to all arena objects
    ArenaObject[] aobjs = GameObject.FindObjectsOfType<ArenaObject>();
    for (int i = 0; i < aobjs.Length; i++)
    {
      aobjs[i].arena_round_stop();
    }

  }

  /// <summary>
  /// launch round end process
  /// called by specific game arena manager
  /// </summary>
  protected void event_round_end()
  {
    Debug.Log("~Arena~ <color=orange>event_round_end</color>");

    setAtState(ArenaState.END);

    //send info to all arena objects
    ArenaObject[] aobjs = GameObject.FindObjectsOfType<ArenaObject>();
    for (int i = 0; i < aobjs.Length; i++)
    {
      aobjs[i].arena_round_end();
    }

    if (coProcessEnd != null)
    {
      Debug.LogError("~Arena~ already processing end ??");
      return;
    }
    
    StartCoroutine(processEnd());
  }

  IEnumerator processEnd()
  {
    IEnumerator ie = process_before_restart();

    Debug.Log("waiting for pre-restart process to be finished ...");

    while (ie.MoveNext()) yield return null;

    Debug.Log("... pre-restart process is <b>done</b> !");
    
    restart_normal();
  }

  /// <summary>
  /// use to describe what to do after round ended, will lead to restart_normal()
  /// </summary>
  virtual protected IEnumerator process_before_restart()
  {
    yield return null;
  }
  
  public float getElapsedTime(){ return round_time; }

  protected void setAtState(ArenaState st)
  {
    Debug.Log("~Arena~ switched state to <b>" + st.ToString()+"</b>");
    _state = st;
  }
  protected ArenaState getState() { return _state; }
  protected bool isAtState(ArenaState st) { return _state == st; }

  public void setArenaIdle() { setAtState(ArenaState.IDLE); }
  public void setArenaLive(){ setAtState(ArenaState.LIVE); }
  public void setArenaMenu() { setAtState(ArenaState.MENU); }
  
  public bool isArenaStateLive() {

    //pause modale kill gameplay
    if (pauseCanvas != null && pauseCanvas.enabled) return false;

    //Debug.Log(_state);

    return liveFreezeTimer <= 0f && isAtState(ArenaState.LIVE);
  }

  public bool isArenaStateEnd() { return isAtState(ArenaState.END); }
  public bool isArenaStateMenu() { return isAtState(ArenaState.MENU); }

  override public string toString()
  {
    string ct = base.toString();
    ct += "\narena objects to update : " + arenaObjects.Count;
    //for (int i = 0; i < arenaObjects.Count; i++) ct += "\n    └ ~" + arenaObjects[i].GetType() + "~ " + arenaObjects[i].name;

    ct += "\n  live freeze timer ? " + liveFreezeTimer;
    ct += "\n  real state : " + _state;

    ct += "\n" + iStringFormatBool("live", isArenaStateLive());
    ct += "\n" + iStringFormatBool("menu", isArenaStateMenu());
    ct += "\n" + iStringFormatBool("end", isArenaStateEnd());
    
    return ct;
  }

  static protected ArenaManager _manager;
  static public ArenaManager get()
  {
    if (_manager == null) _manager = GameObject.FindObjectOfType<ArenaManager>();
    return _manager;
  }
}
