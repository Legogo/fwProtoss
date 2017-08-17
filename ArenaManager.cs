﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class ArenaManager : EngineObject {
  
  public float time = 0f;
  
  virtual public void restart()
  {
    Debug.Log("<b>RESTART</b> at "+Time.time);

    freeze = false;
    
    ArenaObject[] aobjs = GameObject.FindObjectsOfType<ArenaObject>();
    for (int i = 0; i < aobjs.Length; i++)
    {
      aobjs[i].restart();
    }
  }

  protected override void update()
  {
    base.update();
    
    if (Input.GetKeyUp(KeyCode.Space))
    {
      restart();
    }

    //speed up debug
    float mul = 1f;

    if (Input.GetKey(KeyCode.P))
    {
      mul = 100f;
    }

    time += Time.deltaTime * mul;
  }

  public float getElapsedTime()
  {
    return time;
  }

  static protected ArenaManager _manager;
  static public ArenaManager get()
  {
    if (_manager == null) _manager = GameObject.FindObjectOfType<ArenaManager>();
    return _manager;
  }
}
