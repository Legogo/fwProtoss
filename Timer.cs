using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer : ArenaObject {

  public string timerName = "";

  protected SODataTimer balancing;
  
  public bool liveOnStartup = true;
  public bool restartOnTimeout = true;
  public bool liveOnEnd = false;
  public bool timeoutOnPlay = false;

  protected float timer = -1f;
  
  public Action timeout;
  
  public void setupBalancing(SODataTimer bal)
  {
    balancing = bal;
  }

  public void setupAndStart(SODataTimer balancing)
  {
    setupBalancing(balancing);
    startTimer();
  }
  
  public void startTimer()
  {

    if (balancing == null) Debug.LogError("no balancing on " + name, gameObject);
    
    //update interal param
    balancing.fetchTimeParam();

    if (balancing.getCurrentParam() == null) Debug.LogError("no current param ? ", gameObject);

    timer = 0f;
  }

  public void setupForTimeout()
  {
    TimerParams param = balancing.getCurrentParam();
    timer = param.value - 0.0001f;
  }

  public void stop()
  {
    timer = -1f;
  }

  public bool isRunning()
  {
    return timer >= 0f;
  }


  protected override void updateArenaLive(float timeStamp)
  {
    base.updateArenaLive(timeStamp);
    
    //next param time !
    if(balancing != null && balancing.timedParams.Length > 1)
    {
      TimerParams param = balancing.getCurrentParam();

      if (timeStamp > param.timeMark)
      {
        Debug.Log(name + " next param " + timeStamp + " > " + param.timeMark);
      }
    }

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

    TimerParams param = balancing.getCurrentParam();

    if (param == null) return;

    //DEBUG
#if UNITY_EDITOR
    if (Input.GetKeyUp(KeyCode.T))
    {
      Debug.LogWarning("skip timer, debug");
      solveTimeout();
      return;
    }
#endif

    //Debug.Log(name+" | "+timer + " / " + param.value);

    if (timer < param.value)
    {
      timer += Time.deltaTime;
      if(timer > param.value)
      {
        solveTimeout();
      }
    }

  }

  public float getProgress()
  {
    TimerParams param = balancing.getCurrentParam();
    return timer / param.value;
  }

  public void solveTimeout()
  {
    TimerParams param = balancing.getCurrentParam();
    
    //jamais démarré ...
    if (param == null)
    {
      //Debug.LogWarning("no param ? " + timerName, gameObject);
      return;
    }

    //Debug.Log("<b>"+timerName + "</b> solving timeout !", gameObject);

    //loop timer ?
    if (restartOnTimeout) timer = 0f;
    else timer = -1f;
    
    if (timeout != null) timeout();
  }

}
