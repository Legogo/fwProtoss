﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapacityInput : LogicCapacity {

  public InputKeyBridge keys;
  
  public enum InputKeyboardMode { NONE, TOPDOWN };
  public InputKeyboardMode inputKeyboardMode;

  public Action<InputTouchFinger> touch;
  //public Action<InputTouchFinger> release;

  public bool useTouching = false;
  
  protected override void build()
  {
    base.build();

    if(inputKeyboardMode != InputKeyboardMode.NONE)
    {
      keys = new InputKeyBridge();
    }
  }

  public override void earlySetupCapacity()
  {
    base.earlySetupCapacity();
    switch (inputKeyboardMode)
    {
      case InputKeyboardMode.TOPDOWN: keys.create<InputKeyTopDown>(); break;
      default: break;
    }

    if (useTouching)
    {
      subscribeToInput(inputTouch);
    }
  }

  protected void inputTouch(InputTouchFinger finger)
  {
    if (_unfreeze) return;
    if (touch != null) touch(finger);
  }

  /* must be called by owner if need more inputs */
  public T setupKey<T>() where T : InputKey
  {
    keys.create<T>();
    return keys.get<T>();
  }
}
