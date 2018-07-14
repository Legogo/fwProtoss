﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class ArenaManager : EngineObject {
  
  public float round_time = 0f; // round elasped time

  protected float liveFreezeTimer = 0f;

  public enum ArenaState { IDLE, MENU, SETUP_LIVE, LIVE, END }
  protected ArenaState _state = ArenaState.IDLE;

  protected Coroutine coProcessEnd;

  protected Canvas pauseCanvas;

  public List<ArenaObject> arenaObjects = new List<ArenaObject>();

  protected override void build()
  {
    base.build();
    EngineManager.get().onLoadingDone += onLoadingFinished;
  }

  /* this is default behavior for arena manager, it needs to be overrided if need to delay startup of arena */
  virtual protected void onLoadingFinished()
  {
    Debug.Log("~ArenaManager~ loading is finished, calling restart_normal() <- this should be overriten by context");
    restart_normal();
  }

  virtual public void restart_normal() {
    Debug.Log("~<b>Arena</b>~ restart_normal");
    round_time = 0f;
    restart_setup();
  }

  virtual public void restart_setup() {

    for (int i = 0; i < arenaObjects.Count; i++)
    {
      arenaObjects[i].restart();
    }
    
    setFreeze(false);
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

  protected override void setup()
  {
    base.setup();
    fetchPauseCanvasScreen();
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
    }
    else if(_state == ArenaState.END)
    {
      update_end();
    }
    
  }

  virtual protected void update_menu()
  {

  }
  
  protected void update_time()
  {
    round_time += Time.deltaTime;

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

  virtual protected void update_end()
  {

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

  /* end of round ? */
  public void event_end()
  {
    //Debug.Log("~Arena~ event end");

    setAtState(ArenaState.END);

    //send info to all arena objects
    ArenaObject[] aobjs = GameObject.FindObjectsOfType<ArenaObject>();
    for (int i = 0; i < aobjs.Length; i++)
    {
      aobjs[i].event_end();
    }

    if (coProcessEnd != null)
    {
      Debug.LogError("~Arena~ already processing end ??");
      return;
    }
    
    coProcessEnd = StartCoroutine(processEnd());
  }
  
  /* inheritence NEED to set coprocessend = null manually */
  virtual protected IEnumerator processEnd()
  {
    yield return null;

    coProcessEnd = null;
  }
  
  public float getElapsedTime()
  {
    return round_time;
  }

  protected void setAtState(ArenaState st)
  {
    Debug.Log("~Arena~ switched state to <b>" + st.ToString()+"</b>");
    _state = st;
  }
  protected ArenaState getState() { return _state; }
  protected bool isAtState(ArenaState st) { return _state == st; }
  
  public void setArenaLive(){ setAtState(ArenaState.LIVE); }
  public void setArenaMenu() { setAtState(ArenaState.MENU); }
  public void setArenaSetupLive() { setAtState(ArenaState.SETUP_LIVE); }
  
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
