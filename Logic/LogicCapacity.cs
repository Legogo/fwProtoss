using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class LogicCapacity : EngineObject {

  protected LogicItem _owner;
  
  protected override void build()
  {
    base.build();

    _owner = gameObject.GetComponent<LogicItem>();
    if (_owner == null) _owner = gameObject.AddComponent<LogicItem>(); // need

  }

  abstract public void setupCapacity();

  /* must be called by owner ! */
  abstract public void updateLogic();
  
  public LogicItem getOwner() { return _owner; }
}
