using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using fwp.input;

/// <summary>
/// deprecated ; directly use InputKeyBridge
/// </summary>

namespace fwp
{
  abstract public class CapacityInput : LogicCapacity
  {
    /*
    public InputKeyBridge keys;
    
    public Action<InputTouchFinger> touch;
    //public Action<InputTouchFinger> release;

    public bool useTouching = false;

    protected override void build()
    {
      base.build();

      keys = new InputKeyBridge();
    }

    public override void earlySetupCapacity()
    {
      base.earlySetupCapacity();
      
      if (useTouching)
      {
        subscribeToTouchRelease(onTouch);
      }
    }

    protected void onTouch(InputTouchFinger finger)
    {
      if (isFreezed()) return;
      if (touch != null) touch(finger);
    }
    */
  }
}