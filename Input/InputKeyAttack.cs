using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyAttack : InputKey
{
  public bool pressed_attack()
  {
    if (_xinput != null)
    {
      if (_xinput.isPressed(Controller360.ControllerButtons.X)) return true;
      return false;
    }

    return Input.GetKeyDown(KeyCode.LeftShift);
  }

}
