using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperInput {

  static public Vector3 getFingerPosition()
  {
    return Camera.main.ScreenToWorldPoint(Input.mousePosition);
  }
}
