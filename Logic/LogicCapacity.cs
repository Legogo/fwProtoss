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
  abstract public void setupCapacity();
  
  abstract public void clean(); // explain how the module reset

  virtual public void updateLogic() { }
  virtual public void updateLogicLate() { }
  
  public void lockCapacity() { _lock = true; }
  public void unlockCapacity() { _lock = true; }

  public LogicItem getOwner() { return _owner; }
}
