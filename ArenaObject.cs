using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// active
/// spawn
/// update
/// </summary>

public class ArenaObject : EngineObject {

  bool _active = true;

  protected ArenaManager _arena;
  protected InputObject _input;

  protected override void build()
  {
    base.build();

    _arena = ArenaManager.get();

    _input = GetComponent<InputObject>();

    if(_input != null)
    {

      _input.cbTouchOver += onTouch;
      _input.cbReleaseOver += onRelease;

    }

  }

  virtual protected void onTouch(InputTouchFinger finger, RaycastHit2D hit)
  {



  }

  virtual protected void onRelease(InputTouchFinger finger, RaycastHit2D hit)
  {



  }

  virtual public void launch()
  {
    setActive(true);
  }

  virtual public void restart()
  {

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

    setActive(true);

    transform.position = pos;

    spawnProcess(pos);
  }

  virtual protected void spawnProcess(Vector3 position)
  {

  }

  virtual protected void collect()
  {
    kill();
  }

  virtual public void kill()
  {
    setActive(false);

    //Debug.Log(Time.time+" , "+name + " killed", gameObject);
  }

  protected override void update()
  {
    base.update();

    //Debug.Log(name + " active ? " + isActive());

    if (!isActive()) return;
    
    if(_arena != null)
    {
      if (_arena.isFreezed()) return;
      
      if(_arena.isLive()) updateArena(_arena.getElapsedTime());
      else if(_arena.isEnd()) updateArenaEnd();
      else updateArenaMenu();

    }
    

    
  }

  virtual protected void updateArenaMenu()
  {

  }

  virtual protected void updateArenaEnd()
  {

  }

  virtual protected void updateArena(float timeStamp)
  {

    if (isCollectable() && isCollidingWithAvatar())
    {
      collect();
    }

  }

  virtual public bool isCollidingWithAvatar()
  {
    return false;
  }

  virtual public bool isCollectable()
  {
    return isActive();
  }

  protected void setActive(bool flag) { _active = flag; }

  public bool isActive() { return _active; }


}
