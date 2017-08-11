using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer : ArenaObject {

  public string timerName = "";

  public float target = 1f;
  protected float timer = 0f;

  public Action timeout;

  public void reset()
  {
    timer = 0f;
  }

  protected override void updateArena()
  {
    base.updateArena();

    //Debug.Log(name + " " + timer);

    if(timer < target)
    {
      timer += Time.deltaTime;
      if(timer > target)
      {
        timer = 0f;
        if(timeout != null) timeout();
      }
    }
  }
}
