using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyTopDown : InputKey {
  
  public bool pressing_down()
  {
    if (_xinput != null && !pressingAnyHorizontalKey())
    {
      if (_xinput.GetStick(Stick.Left).y < -0.25f) return true;
      return false;
    }

    return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
  }

  public bool pressing_up()
  {
    if (_xinput != null && !pressingAnyHorizontalKey())
    {
      if (_xinput.GetStick(Stick.Left).y > 0.25f) return true;
      return false;
    }

    return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Z);
  }

  public bool pressing_left()
  {
    if (_xinput != null)
    {
      if (Mathf.Abs(_xinput.GetStick(Stick.Left).y) > 0.7f) return false;
      if (_xinput.GetStick(Stick.Left).x < -0.25f) return true;
      return false;
    }
    
    return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q);
  }

  public bool pressing_right()
  {
    if (_xinput != null)
    {
      if (Mathf.Abs(_xinput.GetStick(Stick.Left).y) > 0.7f) return false;
      if (_xinput.GetStick(Stick.Left).x > 0.25f) return true;
      return false;
    }

    return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
  }

  public bool pressed_jump()
  {
    if (_xinput != null)
    {
      if (_xinput.isPressed(Controller360.ControllerButtons.A)) return true;
      return false;
    }

    return Input.GetKeyDown(KeyCode.Space);
  }

  public bool pressingAnyHorizontalKey()
  {
    return pressing_left() || pressing_right();
  }

  public int getDirection()
  {
    if (pressing_left()) return -1;
    else if (pressing_right()) return 1;
    return 0;
  }

}
