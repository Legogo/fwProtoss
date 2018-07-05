using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Some specific capacity that an LogicItem can have
/// It's updated by it's owner
/// </summary>

abstract public class LogicCapacity : EngineObject {

  protected bool _lock;
  protected LogicItem _owner;
  protected CharacterLogic _character;

  public LogicCapacity[] lockDependencies;

  protected override void build()
  {
    base.build();

    _owner = gameObject.GetComponent<LogicItem>();
    //if (_owner == null) Debug.LogError("you NEED to <b>add a LogicItem</b> script to manipulate Capacities on : "+name, gameObject);
    if (_owner == null) _owner = gameObject.AddComponent<LogicItem>(); // need

    _owner.subscribeCapacity(this);

    _character = gameObject.GetComponent<CharacterLogic>();
  }

  /* called by LogicItem, on scene loading */
  virtual public void earlySetupCapacity() { }
  virtual public void setupCapacity() { }
  virtual public void restartCapacity() { }

  /* explain how the module resets */
  virtual public void clean(){ }

  virtual public void updateCapacity() { }
  virtual public void updateCapacityLate() { }
  
  public bool isLocked() { return _lock; }
  public void lockCapacity(bool flag, bool onlyDependencies = false) {

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
