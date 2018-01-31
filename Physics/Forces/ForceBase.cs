using System;
using UnityEngine;

public abstract class ForceBase
{
  private string _name;

  protected GameObject _owner;
  protected CapacityMovement _movement;

  protected bool _instant = false; // only one frame
  protected bool _applied = false; // already applied ? (one frame)

  protected Vector2 _force = Vector2.zero;

  protected ForceBase(string name, bool instant)
  {
    _name = name;
    _instant = instant;
  }

  public void assignOwner(GameObject newOwner)
  {
    _owner = newOwner;
    _movement = _owner.GetComponent<CapacityMovement>();
  }

  public string Name
  {
    get { return _name; }
  }

  bool canApply()
  {
    if (_instant && _applied) return false;
    return true;
  }

  public void update()
  {
    if (!canApply())
    {
      _force.x = _force.y = 0f;
      return;
    }

    if (_instant)
    {
      if (!_applied) _applied = true;
    }

    compute();
  }

  public Vector2 getValue() { return _force; }

  /* descibe what to do when updating */
  abstract protected void compute();
  
  public bool needToBeRemoved()
  {
    return _instant && _applied;
  }
}
