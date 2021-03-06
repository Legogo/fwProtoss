﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class GameTime {

  static public float deltaTime = 0f;
  static public float scale = 1f;

  static public float elapsedTime = 0f;
  static public void addScale(float step)
  {
    scale += step;
    scale = Mathf.Max(0f, scale);
    Debug.LogWarning("time scale is at " + scale);
  }

  /* called by EngineManager (Update) */
  static public void update()
  {
    //update_debug();

    deltaTime = Time.deltaTime * scale;
    elapsedTime += deltaTime;
  }

  static public void update_debug()
  {

    if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyUp(KeyCode.M)) addScale(1f); // change time scale
    else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyUp(KeyCode.L)) addScale(-1f); // change time scale

    /*
    if (Input.GetMouseButton(1)) scale = 5f;
    else scale = 1f;
    */

  }
}
