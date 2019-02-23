﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// onLoadingFinished() need to be override by context !
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

  public bool debug_useFocusPause = false;

  public float round_time = 0f; // round elasped time

  protected float liveFreezeTimer = 0f;

  public enum ArenaState { IDLE, MENU, LIVE, ROUND_PAUSE, END }
  protected ArenaState _state = ArenaState.IDLE;

  protected Coroutine coProcessEnd;
  
  public List<ArenaObject> arenaObjects = new List<ArenaObject>();
  
  protected override void setupLate()
  {
    base.setupLate();

    EngineEventSystem.onPauseEvent += onSystemPause;
    EngineEventSystem.onFocusEvent += onFocusPause;

    if (inDebugContext())
    {
      Debug.LogWarning("arena is starting up auto in debug context");
      arena_startup(); // startup in debug context
    }
  }
  
  virtual protected bool inDebugContext()
  {
    return !EngineLoader.isGameScene();
  }
  
  protected void onFocusPause(bool state)
  {
    if (!debug_useFocusPause) return;

    Debug.Log(getStamp() + " focus pause ? " + state);
    onSystemPause(!state);
  }

  protected void onSystemPause(bool state)
  {
    Debug.Log(getStamp() + " system pause ? " + state);

    if (state && canPause()) onRoundPause(true);
    else if(!state && canUnpause()) onRoundPause(false);
    
  }

  /// <summary>
  /// external call for pause (not device event)
  /// </summary>
  /// <param name="state"></param>
  public void callForPause(bool state)
  {
    Debug.Log(getStamp() + " external call for pause : "+state);
    onSystemPause(state);
  }

  /// <summary>
  /// permet de dire a tout les AO qu'on appelle une pause spécifique a l'arene
  /// </summary>
  /// <param name="state"></param>
  virtual protected void onRoundPause(bool state)
  {
    if(state && _state == ArenaState.ROUND_PAUSE) Debug.LogWarning("paused called but arena is already at pause state");
    if(!state && _state == ArenaState.LIVE) Debug.LogWarning("paused called but arena is already at pause state");
    
    if (state) _state = ArenaState.ROUND_PAUSE; // a menu poped and is iterruting gameplay ?
    else _state = ArenaState.LIVE; // come back to live gameplay

    //Debug.Log("arena onRoundPause "+state+" --> arena tat")

    Debug.Log(getStamp() + "round pause ("+state+") | arena state switched to "+_state);

    for (int i = 0; i < arenaObjects.Count; i++)
    {
      arenaObjects[i].round_pause(state);
    }
  }

  /// <summary>
  /// describe how to clean (destroy everything)
  /// default : called when going to home menu
  /// </summary>
  virtual public void arena_cleanup()
  {
    for (int i = 0; i < arenaObjects.Count; i++)
    {
      arenaObjects[i].arena_cleanup();
    }

    HalperNatives.clearGC();
  }

  /// <summary>
  /// this is entry point for session generation / startup
  /// called auto in debug context (not game scene)
  /// some object need to create/regenerate stuff when coming back to arena (if ld data changed)
  /// must be wrapped for more readability
  /// </summary>
  virtual protected void arena_startup()
  {
    arena_cleanup();

    //if something calls the restart but the process is doing end stuff
    cancelEndProcess();

    Debug.Log(getStamp() + "arena startup");

    for (int i = 0; i < arenaObjects.Count; i++)
    {
      arenaObjects[i].arena_startup();
    }

    round_restart();

    //Debug.Log(getStamp() + "is calling ingame screen");
    //ScreensManager.open(ScreensManager.ScreenNames.ingame);
  }

  /// <summary>
  /// describe how to setup a restart
  /// </summary>
  virtual protected void round_restart() {

    Debug.Log(getStamp() + "round restart");

    round_time = 0f;

    for (int i = 0; i < arenaObjects.Count; i++)
    {
      arenaObjects[i].arena_round_restart();
    }

    //_state = ArenaState.LIVE;
    onRoundPause(false); // this will make state to live AND tell all AO to stop pausing
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
    for (int i = 0; i < arenaObjects.Count; i++)
    {
      if (!arenaObjects[i].isFreezed()) arenaObjects[i].updateArena();
    }
  }

  virtual protected void update_round_late()
  {
    for (int i = 0; i < arenaObjects.Count; i++)
    {
      if (!arenaObjects[i].isFreezed()) arenaObjects[i].updateArenaLate();
    }
  }

  virtual protected void update_round_end()
  {
    for (int i = 0; i < arenaObjects.Count; i++)
    {
      if (!arenaObjects[i].isFreezed()) arenaObjects[i].updateArena();
    }
  }
  
  /// <summary>
  /// called if system need to interrupt gameplay
  /// </summary>
  virtual public void round_stop()
  {
    _state = ArenaState.IDLE;
    
    //send info to all arena objects
    ArenaObject[] aobjs = GameObject.FindObjectsOfType<ArenaObject>();
    for (int i = 0; i < aobjs.Length; i++)
    {
      aobjs[i].arena_round_stop();
    }

  }

  /// <summary>
  /// launch round end process
  /// called when checkRoundEnd returns true
  /// </summary>
  private void event_round_end()
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
    IEnumerator ie = process_round_end();

    Debug.Log("waiting for pre-restart process to be finished ...");

    while (ie.MoveNext()) yield return null;

    Debug.Log("... pre-restart process is <b>done</b> !");

    round_restart();
  }

  /// <summary>
  /// use to describe what to do after round ended, will lead to restart_normal()
  /// </summary>
  virtual protected IEnumerator process_round_end()
  {
    yield return null;
  }
  
  public float getElapsedTime(){ return round_time; }

  protected void setAtState(ArenaState st)
  {
    Debug.Log(getStamp()+" switched state to <b>" + st.ToString()+"</b>");
    _state = st;
  }
  public void cancelEndProcess()
  {
    if (coProcessEnd != null)
    {
      Debug.LogWarning("process end of arena was active, stopping it ...");
      StopCoroutine(coProcessEnd);
    }
  }
  
  public bool isArenaStateLive() {
    
    //pause will freeze this EO, is managed by onPause() linked to EngineEventSystem

    return liveFreezeTimer <= 0f && isAtState(ArenaState.LIVE);
  }

  public bool isArenaStateEnd() { return isAtState(ArenaState.END); }
  public bool isArenaStateMenu() { return isAtState(ArenaState.MENU); }

  public void setArenaToMenuState() { _state = ArenaState.MENU; }

  public ArenaState getState() { return _state; }
  public bool isAtState(ArenaState st) { return _state == st; }


  virtual protected bool canPause() { return _state == ArenaState.LIVE; }
  virtual protected bool canUnpause() { return _state == ArenaState.ROUND_PAUSE; }

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

  static public string getStamp()
  {
    return "<color=lime>ArenaManager</color> | ";
  }

  static protected ArenaManager _manager;
  static public ArenaManager get() { return get<ArenaManager>(); }
  static public T get<T>() where T : ArenaManager
  {
    if (_manager == null) _manager = GameObject.FindObjectOfType<T>();
    return (T)_manager;
  }
}
