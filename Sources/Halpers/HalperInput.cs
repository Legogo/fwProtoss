using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperInput {

  /// <summary>
  /// returns mouse position in world coords
  /// </summary>
  /// <returns></returns>
  static public Vector3 getMouseToWorldPosition()
  {
    return Camera.main.ScreenToWorldPoint(Input.mousePosition);
  }
}
