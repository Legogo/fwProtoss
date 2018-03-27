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

  virtual public void updateLogic() { }
  virtual public void updateLogicLate() { }
  
  public bool isLocked() { return _lock; }
  public void lockCapacity() { _lock = true; Debug.Log(GetType() + " is now locked"); }
  public void unlockCapacity() { _lock = false; Debug.Log(GetType() + " is now unlocked"); }

  public LogicItem getOwner() { return _owner; }
}
