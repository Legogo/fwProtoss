﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using fwp.input;

namespace fwp
{
  public class CapacityInput : LogicCapacity
  {

    public InputKeyBridge keys;

    public enum InputKeyboardMode { NONE, TOPDOWN };
    public InputKeyboardMode inputKeyboardMode;

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

      switch (inputKeyboardMode)
      {
        case InputKeyboardMode.TOPDOWN: keys.get<InputKeyTopDown>(); break;
        default: break;
      }

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

  }
}