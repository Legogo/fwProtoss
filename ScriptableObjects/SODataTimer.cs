﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "data/DataTimer", order = 100)]
public class SODataTimer : ScriptableObject
{
  protected TimerParams param;
  public TimerParams[] timedParams;
  
  public bool isCurrentParamTimeOver()
  {
    return param.timeMark < ArenaManager.get().getElapsedTime();
  }

  public void fetchTimeParam()
  {
    fetchTimeParam(ArenaManager.get().getElapsedTime());
  }

  public void fetchTimeParam(float elapsed)
  {
    param = getParam(elapsed);
  }

  public TimerParams getCurrentParam()
  {
    if (param == null) fetchTimeParam(ArenaManager.get().getElapsedTime());
    return param;
  }

  protected TimerParams getParam(float timeStamp)
  {
    if (timedParams.Length <= 0)
    {
      Debug.LogWarning("asking for param but timParams[] (of "+name+") is empty");
      return null;
    }

    //return only
    if (timedParams.Length == 1) return timedParams[0];

    //search for timed one
    for (int i = timedParams.Length - 1; i >= 0; i--)
    {
      if (timeStamp > timedParams[i].timeMark) return timedParams[i];
    }

    //return last
    return timedParams[timedParams.Length - 1];
  }

}

[Serializable]
public class TimerParams
{
  public float timeMark;
  public float value;
}