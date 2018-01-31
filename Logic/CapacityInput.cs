using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapacityInput : LogicCapacity {

  public XinputController controller;
  public InputKeyBridge keys;

  public override void clean()
  {
    controller = null;
  }

  public bool isControlled()
  {
    return controller != null;
  }

  public override void setupCapacity(){}

  protected override void build()
  {
    base.build();
    keys = new InputKeyBridge();
  }
  
}
