using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Some specific capacity that an LogicItem can have
/// It's updated by it's owner
/// </summary>

abstract public class LogicCapacity : EngineObject {

  protected LogicItem _owner;
  
  protected override void build()
  {
    base.build();

    _owner = gameObject.GetComponent<LogicItem>();
    if (_owner == null) _owner = gameObject.AddComponent<LogicItem>(); // need

  }

  /* called by LogicItem */
  abstract public void setupCapacity();
  abstract public void updateLogic();
  
  public LogicItem getOwner() { return _owner; }
}
