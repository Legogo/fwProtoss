using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer : ArenaObject {

  public string timerName = "";

  public TimerParams[] timeParams;

  protected float timer = 0f;
  
  public Action timeout;
  
  protected override void build()
  {
    base.build();

    if (timeParams.Length <= 0) Debug.LogError("no time setup for " + name, gameObject);
  }

  public void reset()
  {
    timer = 0f;
  }

  protected override void updateArena(float timeStamp)
  {
    base.updateArena(timeStamp);

    //Debug.Log(name + " " + timer);

    TimerParams param = getParam(timeStamp);

    if (param == null) return;

    if (Input.GetKeyUp(KeyCode.T))
    {
      Debug.LogWarning("skip timer, debug");
      timer = param.value - 0.1f;
    }

    if (timer < param.value)
    {
      timer += Time.deltaTime;
      if(timer > param.value)
      {
        timer = 0f;
        if(timeout != null) timeout();
      }
    }

  }

  protected TimerParams getParam(float timeStamp)
  {
    if (timeParams.Length <= 0) return null;

    for (int i = timeParams.Length - 1; i >= 0; i--)
    {
      if (timeStamp > timeParams[i].timeMark) return timeParams[i];
    }

    return timeParams[timeParams.Length-1];
  }
  
}


[Serializable]
public class TimerParams
{
  public float timeMark;
  public float value;
}