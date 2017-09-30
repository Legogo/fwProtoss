using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime {

  static public float elapsedTime = 0f;

  static public void update()
  {
    elapsedTime += Time.deltaTime;
  }

}
