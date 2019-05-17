using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// 0 -> inf
/// </summary>

public class Timer : TimeObject {
  
  override public float getTimeRemaining()
  {
    return 0f;
  }
  
}
