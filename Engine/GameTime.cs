using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour {

  static public float deltaTime = 0f;
  static public float scale = 1f;

  static public float elapsedTime = 0f;

  static GameTime _instance;

  void FixedUpdate() {

    if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyUp(KeyCode.M)) addScale(1f);
    else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyUp(KeyCode.L)) addScale(-1f);
    
    update();
  }

  static protected void addScale(float step)
  {
    scale += step;
    scale = Mathf.Max(0f, scale);
    Debug.LogWarning("time scale is at " + scale);
  }

  static public void update()
  {
    deltaTime = Time.deltaTime * scale;
    elapsedTime += deltaTime;
  }
}
