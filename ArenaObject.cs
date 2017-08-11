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
  
  virtual public void launch()
  {
    setActive(true);
  }

  virtual public void restart()
  {
  }

  virtual public void spawn(Vector3 position)
  {
    setActive(true);

    transform.position = position;
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

    if (ArenaSnakeManager.manager.freezeArena) return;

    if (!isActive()) return;

    updateArena();
  }

  virtual protected void updateArena()
  {

    if (isCollidingWithAvatar())
    {
      collect();
    }

  }

  virtual public bool isCollidingWithAvatar()
  {
    return false;
  }

  protected void setActive(bool flag) { _active = flag; }

  public bool isActive() { return _active; }

  public float getSize() { return transform.localScale.x; }

}
