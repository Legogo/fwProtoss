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

  public void setupAndStart(SODataTimer balancing = null)
  {
    setupBalancing(balancing);
    startTimer();
  }
  
  virtual public void startTimer()
  {
    //if (balancing == null) Debug.LogError("no balancing for " + name, gameObject);

    //unfreeze, activate
    launch();

    if(balancing != null)
    {
      //update interal param
      balancing.fetchTimeParam();
    }

    //Debug.Log("timer is at 0");
    timer = 0f;
  }

  public void setupForTimeout()
  {
    TimerParams param = balancing.getCurrentParam();
    if(param != null)
    {
      timer = param.value - 0.0001f;
    }
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

    //Debug.Log(timerName + " update "+isRunning());

    if (!isRunning()) return;

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

    TimerParams param = null;
    if (balancing != null) param = balancing.getCurrentParam();
    
    //chrono
    if (param == null)
    {
      timer += Time.deltaTime;
      return;
    }

    //timer
    if (timer < param.value)
    {
      timer += Time.deltaTime;

      //Debug.Log(timerName+" | "+timer +" / "+param.value);

      if (timer > param.value)
      {
        solveTimeout();
      }
    }

  }

  public bool isChrono() { return balancing == null; }

  public float getTime()
  {
    return timer;
  }

  public float getTimeRemaining()
  {
    //Debug.Log(getTargetStep() + " * " + getProgress());
    //float max = getTargetStep();
    if (isChrono()) return timer;

    return getTargetStep() - timer;
  }

  public float getTargetStep()
  {
    if (balancing == null) return 0f;

    TimerParams param = balancing.getCurrentParam();

    if(param != null)
    {
      return param.value;
    }

    return 0f;
  }

  public float getProgress()
  {
    TimerParams param = balancing.getCurrentParam();
    return timer / param.value;
  }

  public void solveTimeout()
  {
    //if (balancing == null) Debug.LogError(name + " no balancing ? ", gameObject);

    //si le timer a jamais été call et qu'on arrive a la fin d'un round
    if (balancing == null) return;

    TimerParams param = balancing.getCurrentParam();
    
    if (param == null) return;

    //Debug.Log(name + " timeout !", gameObject);

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
