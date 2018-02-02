using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// active
/// spawn
/// update
/// </summary>

abstract public class ArenaObject : EngineObject {

  bool _active = true;
  bool _collectable = false;

  protected ArenaManager _arena;
  protected ArenaObjectSettings ao_settings;

  protected override void fetchGlobal()
  {
    base.fetchGlobal();

    //if(name.Contains("timer_")) Debug.Log("<b>" + name + "." + GetType() + "</b> fetchData");
    
    _arena = ArenaManager.get();

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

  protected void subscribeToArenaSettings()
  {
    ao_settings = new ArenaObjectSettings(transform);
  }

  protected override void destroy()
  {
    base.destroy();
    if (_arena == null) return;
    _arena.arenaObjects.Remove(this);
  }
  
  /* contextuel */
  virtual public void launch()
  {

    setFreeze(false);
    setActive(true);

  }

  /* must be called by something */
  virtual public void restart()
  {
    if (ao_settings != null) ao_settings.applyRespawn();
  }
  
  virtual public void event_end()
  {
  }

  public void spawn(Vector3? position)
  {
    if (position == null)
    {
      Debug.LogWarning("Object "+name+" has no given position for spawn", gameObject);
      return;
    }

    Vector3 pos = (Vector3)position;
    transform.position = pos;

    //Debug.Log(name + " spawn");

    setActive(true);
    
    spawnProcess(pos);
    return;
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
    setActive(false);

    //Debug.Log(Time.time+" , "+name + " killed", gameObject);
  }

  virtual public void updateMenu()
  {

  }

  /* called by arena manager */
  virtual public void updateArena()
  {
    //Debug.Log(name + " , active ? " + _active + " ,  freeze ? " + isFreezed());

    if (!_active) return;
    if (isFreezed()) return;

    //Debug.Log(_arena.isArenaStateLive());

    if (_arena.isArenaStateLive())
    {
      updateArenaLive(_arena.getElapsedTime());
    }
    else if (_arena.isArenaStateEnd())
    {
      updateArenaEnd();
    }
    else
    {
      updateArenaMenu();
    }
    
  }

  virtual protected void updateArenaMenu()
  {

  }

  virtual protected void updateArenaEnd()
  {

  }

  virtual protected void updateArenaLive(float timeStamp)
  {
    //Debug.Log(name);
  }

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
    return _collectable && isActive();
  }

  protected void setActive(bool flag) { _active = flag; }

  public bool isActive() { return _active; }

  public override string toString()
  {
    string ct = base.toString();

    ct += "\n  " + iStringFormatBool("active", _active);

    if(_arena != null)
    {
      ct += "\n~arena~";
      ct += "\n  " + iStringFormatBool("arena", _arena != null);
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
