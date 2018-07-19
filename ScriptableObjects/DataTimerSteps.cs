using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "protoss/create DataTimerSteps", order = 100)]
public class DataTimerSteps : DataTimer
{
  //public TimerParams[] timedParams;
  public TimerParam[] timerParamSteps;

  /* update current param based on arena timer */
  public TimerParam fetchTimeParam()
  {
    return getParam(ArenaManager.get().getElapsedTime()).Value;
  }

  public bool hasMultipleParams() { return timerParamSteps.Length > 1; }
  public bool hasParam() { return timerParamSteps.Length > 0; }

  protected TimerParam? getParam(float timeStamp)
  {
    if (timerParamSteps.Length <= 0)
    {
      Debug.LogError("asking for param but timParams[] (of " + name + ") is empty");
      return null;
    }

    //return only
    if (timerParamSteps.Length == 1) return timerParamSteps[0];

    //search for timed one
    for (int i = timerParamSteps.Length - 1; i >= 0; i--)
    {
      if (timeStamp > timerParamSteps[i].timeMark) return timerParamSteps[i];
    }

    //return last
    return timerParamSteps[timerParamSteps.Length - 1];
  }

}

[Serializable]
public class TimerParams
{
  public float timeMark = 0f;
  public float value = 0f;
}

[Serializable]
public struct TimerParam
{
  public float timeMark;
  public float value;
}