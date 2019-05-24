using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Some specific capacity that an LogicItem can have
/// It's updated by it's owner
/// </summary>

namespace fwp
{
  abstract public class LogicCapacity : EngineObject
  {

    protected bool _lock;
    protected LogicItem _owner;
    protected CharacterLogic _character;

    [HideInInspector]
    public LogicCapacity[] lockDependencies;

    protected override void build()
    {
      base.build();

      _owner = gameObject.GetComponent<LogicItem>(); // local
      if (_owner == null) _owner = gameObject.GetComponentInParent<LogicItem>(); // in parent ?

      if (_owner == null)
      {
        Debug.LogWarning("creating owner");
        _owner = gameObject.AddComponent<LogicItem>(); // if none
      }

      //if (_owner == null) Debug.LogError("you NEED to <b>add a LogicItem</b> script to manipulate Capacities on : "+name, gameObject);

      _owner.subscribeCapacity(this);

      _character = gameObject.GetComponent<CharacterLogic>();
    }

    sealed public override void updateEngine()
    {
      base.updateEngine();
    }

    /* called by LogicItem, on scene loading */
    virtual public void earlySetupCapacity() { }
    virtual public void setupCapacity() { }

    /// <summary>
    /// called on arena round_restart
    /// </summary>
    virtual public void restartCapacity() { }

    /* explain how the module resets */
    virtual public void clean() { }

    virtual public void updateCapacity() { }
    virtual public void updateCapacityLate() { }

    /// <summary>
    /// is capacity locked by another (sort of freeze)
    /// </summary>
    public bool isLocked() { return _lock; }
    public void lockCapacity(bool flag, bool onlyDependencies = false)
    {

      if (onlyDependencies)
      {
        setupLockDependencies(flag);
        return;
      }

      // /w\ inf loop !
      if (_lock != flag)
      {
        //Debug.Log("locking ? " + flag + " capacity : " + GetType()+" and "+lockDependencies.Length+" other dep");
        _lock = flag;
        setupLockDependencies(_lock);
      }

    }

    protected void setupLockDependencies(bool flag)
    {
      if (lockDependencies != null && lockDependencies.Length > 0)
      {
        //Debug.Log("also locking " + lockDependencies.Length + " other capacities");

        for (int i = 0; i < lockDependencies.Length; i++)
        {
          lockDependencies[i].lockCapacity(flag);
        }
      }

    }

    public LogicItem getOwner() { return _owner; }
  }

}
