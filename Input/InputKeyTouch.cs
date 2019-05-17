using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.input
{
  public class InputKeyTouch : InputKey
  {
    public bool touch() { return false; }
    public bool release() { return false; }
  }
}