using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyAttack : InputKey
{
  public bool pressed_attack()
  {
    return Input.GetKeyDown(KeyCode.LeftShift);
  }

}
