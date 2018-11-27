using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// meant to react to arenamanager behavior (restart, update, end of round, ...)
/// </summary>

abstract public class ArenaObject : EngineObject {
  
  bool _collectable = false;

  protected ArenaManager _arena;
  protected ArenaObjectSettings ao_settings;

  protected override void build()
  {
    base.build();
    _arena = ArenaManager.get();
  }

  protected override void setupEarly()
  {
    base.setupEarly();
    _arena = ArenaManager.get();
  }
  
  protected override void setup()
  {
    base.setup();
    
    //resource ref must NOT be updated by arena
    if (gameObject.CompareTag("resource")) return;

    if (_arena != null)
    {
      if (_arena.arenaObjects.IndexOf(this) < 0) _arena.arenaObjects.Add(this);
    }
    else Debug.LogWarning(name + " no arena ?", gameObject);
    
  }

  protected void subscribeToCollectable()
  {
    _collectable = true;
  }

  /* record some settings to apply on next restart (spawn position, etc) */
  protected void subscribeToArenaSettings()
  {
    ao_settings = new ArenaObjectSettings(transform);
  }

  /* contextuel */
  virtual public void launch()
  {
    //arena overrides freeze state of engine object !
    setFreeze(false);
  }

  /// <summary>
  /// called before restart
  /// </summary>
  virtual public void arena_startup()
  {

  }

  virtual public void round_pause(bool pauseState)
  {
    setFreeze(pauseState);
  }

  /// <summary>
  /// called by ArenaManager on round restart
  /// </summary>
  virtual public void arena_round_restart()
  {
    if (ao_settings != null) ao_settings.applyRespawn();
  }

  virtual public void arena_round_stop()
  {

  }
  
  protected override void destroy()
  {
    base.destroy();
    if (_arena == null) return;
    _arena.arenaObjects.Remove(this);
  }

  /// <summary>
  /// called by ArenaManager
  /// </summary>
  virtual public void arena_round_end()
  {
  }

  public void spawn(Vector3 position)
  {
    transform.position = position;

    //Debug.Log(name + " spawn at "+ transform.position);

    setFreeze(false);
    
    spawnProcess(transform.position);
  }

  virtual protected void spawnProcess(Vector3 position)
  {

  }

  virtual protected void collect()
  {
    explode();
  }

  virtual public void explode() {
    kill();
  }

  virtual public void kill()
  {
    setFreeze(true);
    //Debug.Log(Time.time+" , "+name + " killed", gameObject);
  }
  
  /// <summary>
  /// called by arena manager
  /// </summary>
  public void updateArena()
  {
    if (EngineManager.isPaused()) return;

    if (_arena.isArenaStateLive()) updateArenaLive(_arena.getElapsedTime());
    else if (_arena.isArenaStateEnd()) updateArenaEnd();
    else updateArenaMenu();
  }

  public void updateArenaLate()
  {
    if (_arena.isArenaStateLive()) updateArenaLiveLate(_arena.getElapsedTime());
  }

  virtual protected void updateArenaMenu()
  {

  }

  virtual protected void updateArenaEnd()
  {

  }

  virtual protected void updateArenaLive(float timeStamp){}
  virtual protected void updateArenaLiveLate(float timeStamp){}

  /* déclanche le process de collect quand l'objet touche l'avatar */
  protected void update_collectable()
  {
    if (!isCollectable()) return;
    if (isCollidingWithAvatar()) collect();
  }

  virtual public bool isCollidingWithAvatar()
  {
    return false;
  }

  virtual public bool isCollectable()
  {
    return _collectable;
  }
  
  public override string toString()
  {
    string ct = base.toString();

    ct += "\n~ArenaObject~";

    if (_arena != null)
    {
      ct += "\n  " + iStringFormatBool("arena != null", _arena != null);
      ct += "\n  " + iStringFormatBool("arena live", _arena.isArenaStateLive());
      ct += "\n  " + iStringFormatBool("arena end", _arena.isArenaStateEnd());
    }

    if (isCollectable())
    {
      ct += "\n  collectable !";
      ct += "\n  " + iStringFormatBool("base. colliding /w avatar", isCollidingWithAvatar());
    }
    
    return ct;
  }

}
