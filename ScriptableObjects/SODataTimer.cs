using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "data/DataTimer", order = 100)]
public class SODataTimer : ScriptableObject
{
  
  public TimerParams[] timerParams;
  
  protected TimerParams param;
  protected TimerParams[] timeParams;

  public bool isCurrentParamTimeOver()
  {
    return param.timeMark < ArenaManager.get().getElapsedTime();
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
    if (timeParams.Length <= 0) return null;

    for (int i = timeParams.Length - 1; i >= 0; i--)
    {
      if (timeStamp > timeParams[i].timeMark) return timeParams[i];
    }

    return timeParams[timeParams.Length - 1];
  }

}

[Serializable]
public class TimerParams
{
  public float timeMark;
  public float value;
}