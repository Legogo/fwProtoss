using System.Collections;
using System.Collections.Generic;
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
  
  public float round_time = 0f; // round elasped time

  protected float liveFreezeTimer = 0f;

  public enum ArenaState { IDLE, MENU, LIVE, END }
  protected ArenaState _state = ArenaState.IDLE;

  protected Coroutine coProcessEnd;
  
  public List<ArenaObject> arenaObjects = new List<ArenaObject>();

  protected override void build()
  {
    base.build();
    
    EngineEventSystem.onPause += onPause;
  }

  virtual protected void onPause(bool state)
  {
    setFreeze(state);
  }
  
  protected override void setupLate()
  {
    base.setupLate();

    startup();
  }

  /// <summary>
  /// by default called on setup(), can be overrided to not start round by default
  /// </summary>
  virtual protected void startup()
  {
    Debug.Log(getStamp() + "DEFAULT BEHAVIOR | starting round right away");

    arena_startup();
  }
  
  /// <summary>
  /// what must be called by a menu to start the first round
  /// some object need to create/regenerate stuff when coming back to arena (if ld data changed)
  /// </summary>
  virtual public void arena_startup()
  {
    //if something calls the restart but the process is doing end stuff
    cancelEndProcess();

    Debug.Log(getStamp() + "arena startup");

    for (int i = 0; i < arenaObjects.Count; i++)
    {
      arenaObjects[i].arena_startup();
    }

    round_restart();
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
    updateArenaObjects(false);
  }

  virtual protected void update_round_late()
  {
    updateArenaObjects(true);
  }

  virtual protected void update_round_end()
  {
    updateArenaObjects(false);
  }

  protected void updateArenaObjects(bool late = false)
  {
    for (int i = 0; i < arenaObjects.Count; i++)
    {
      if (arenaObjects[i].isFreezed()) continue;
      if (late) arenaObjects[i].updateArenaLate();
      else arenaObjects[i].updateArena();
    }
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
    IEnumerator ie = process_before_restart();

    Debug.Log("waiting for pre-restart process to be finished ...");

    while (ie.MoveNext()) yield return null;

    Debug.Log("... pre-restart process is <b>done</b> !");

    round_restart();
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
    Debug.Log(getStamp()+" switched state to <b>" + st.ToString()+"</b>");
    _state = st;
  }
  protected ArenaState getState() { return _state; }
  protected bool isAtState(ArenaState st) { return _state == st; }

  public void cancelEndProcess()
  {
    if (coProcessEnd != null)
    {
      Debug.LogWarning("process end of arena was active, stopping it ...");
      StopCoroutine(coProcessEnd);
    }
  }

  public void setArenaIdle() { setAtState(ArenaState.IDLE); }
  public void setArenaLive(){ setAtState(ArenaState.LIVE); }
  public void setArenaMenu() { setAtState(ArenaState.MENU); }
  
  public bool isArenaStateLive() {
    
    //pause will freeze this EO, is managed by onPause() linked to EngineEventSystem

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

  static public string getStamp()
  {
    return "<color=lime>ArenaManager</color> | ";
  }

  static protected ArenaManager _manager;
  static public ArenaManager get()
  {
    if (_manager == null) _manager = GameObject.FindObjectOfType<ArenaManager>();
    return _manager;
  }
}
