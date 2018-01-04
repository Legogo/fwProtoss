using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyTopDown : InputKey {
  
  public bool press_down()
  {
    return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
  }

  public bool press_up()
  {
    return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Z);
  }

  public bool press_left()
  {
    return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q);
  }

  public bool press_right()
  {
    return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
  }
}
