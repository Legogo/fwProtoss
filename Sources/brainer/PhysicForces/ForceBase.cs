using System;
using UnityEngine;

namespace brainer
{
  public abstract class ForceBase
  {
    private string _name;
    private bool _isActive = true;

    protected GameObject _owner;
    protected capacity.CapacityMovement _movement;

    protected bool _once = false; // only one frame
    protected bool _applied = false; // already applied ? (one frame)

    protected Vector2 _force = Vector2.zero;

    protected ForceBase(string name, bool appliedOnce)
    {
      _name = name;
      _once = appliedOnce;
    }

    public bool IsActive
    {
      get { return _isActive; }
      set { _isActive = value; }
    }

    public void assignOwner(GameObject newOwner)
    {
      _owner = newOwner;
      _movement = _owner.GetComponent<capacity.CapacityMovement>();
    }

    public string Name
    {
      get { return _name; }
    }

    bool canApply()
    {
      if (_once && _applied) return false;
      return true;
    }

    public void update()
    {
      if (!canApply())
      {
        _force.x = _force.y = 0f;
        return;
      }

      if (_once)
      {
        if (!_applied)
        {
          _applied = true;
          //Debug.Log(Name + " is done applying");
        }
      }

      compute();
    }

    public Vector2 getValue() { return IsActive ? _force : Vector2.zero; }

    /* descibe what to do when updating */
    abstract protected void compute();

    virtual public bool needToBeRemoved()
    {
      return _once && _applied;
    }
  }

}
