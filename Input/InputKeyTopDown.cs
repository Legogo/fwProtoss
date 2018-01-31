using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyTopDown : InputKey {
  
  public bool pressing_down()
  {
    return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
  }

  public bool pressing_up()
  {
    return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Z);
  }

  public bool pressing_left()
  {
    return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q);
  }

  public bool pressing_right()
  {
    return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
  }

  public bool pressed_jump()
  {
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
