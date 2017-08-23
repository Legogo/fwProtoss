using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer : ArenaObject {

  public string timerName = "";

  public TimerParams[] timeParams;

  public bool liveOnStartup = true;
  public bool restartOnTimeout = true;
  public bool liveOnEnd = false;

  protected float timer = -1f;
  
  public Action timeout;
  
  protected override void build()
  {
    base.build();

    if (timeParams.Length <= 0) Debug.LogError("no time setup for " + name, gameObject);
  }

  public override void restart()
  {
    base.restart();

    if (liveOnStartup)
    {
      play();
    }
  }

  public void play()
  {
    timer = 0f;
  }

  public void stop()
  {
    timer = -1f;
  }

  public bool isRunning()
  {
    return timer >= 0f;
  }


  protected override void updateArena(float timeStamp)
  {
    base.updateArena(timeStamp);
    updateTimer();
  }
  protected override void updateArenaEnd()
  {
    base.updateArenaEnd();

    if (!liveOnEnd) return;

    //Debug.Log("timer "+timerName+" updateEnd | ? "+timer+" / "+getTarget(), gameObject);

    updateTimer();
  }

  protected void updateTimer() {

    if (!isRunning()) return;

    //Debug.Log(name + " " + timer);

    //fetch param
    TimerParams param = getCurrentParam();
    if (param == null) return;

    //DEBUG
    if (Input.GetKeyUp(KeyCode.T))
    {
#if UNITY_EDITOR
      Debug.LogWarning("skip timer, debug");
#endif
      setAtEnd();
    }

    //Debug.Log(timer + " / " + param.value);

    if (timer < param.value)
    {
      timer += Time.deltaTime;
      if(timer > param.value)
      {
        //loop timer ?
        if (restartOnTimeout) timer = 0f;
        else timer = -1f;

        if (timeout != null)
        {
          //Debug.Log(timerName + " timeout !", gameObject);
          timeout();
        }
      }
    }

  }

  public void setAtEnd()
  {
    TimerParams param = getCurrentParam();
    timer = param.value - 0.1f;
  }

  protected float getTarget() { return getCurrentParam().value; }

  protected TimerParams getCurrentParam()
  {
    return getParam(ArenaManager.get().getElapsedTime());
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