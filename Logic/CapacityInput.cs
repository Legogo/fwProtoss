using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapacityInput : LogicCapacity {

  public InputKeyBridge keys;

  public override void clean(){}

  public override void setupCapacity(){}

  protected override void build()
  {
    base.build();
    keys = new InputKeyBridge();
  }
  
}
