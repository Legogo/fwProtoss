using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyAim : InputKey {

  public bool pressed_aim()
  {
    if (_xinput != null)
    {
      if (_xinput.isPressed(Controller360.ControllerButtons.RB)) return true;
      else if (_xinput.isPressed(Controller360.ControllerButtons.LB)) return true;
      return false;
    }

    return Input.GetKeyDown(KeyCode.A);
  }
  public bool pressed_release()
  {
    if (_xinput != null)
    {
      if (_xinput.isReleased(Controller360.ControllerButtons.RB)) return true;
      else if (_xinput.isReleased(Controller360.ControllerButtons.LB)) return true;
      return false;
    }

    return Input.GetKeyUp(KeyCode.A);
  }
  
}
