using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
    //if(balancing == null) Debug.LogWarning(timerName + " setupBalancing(null)", gameObject);
  }

  public void setupAndStart(SODataTimer balancing = null)
  {
    //if (balancing == null) Debug.Log("balancing is null ?");

    setupBalancing(balancing);
    startTimer();
  }
  
  virtual public void startTimer()
  {

#if UNITY_EDITOR
    //if (balancing != null) Debug.Log("timer " + timerName + " starting as timeout. Timeout in " + balancing.getCurrentParam().value, gameObject);
    //else Debug.Log("timer " + timerName + " starting as a chrono (no balancing / timeout)", gameObject);
#endif

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
    if (balancing == null) return;

    TimerParams param = balancing.getCurrentParam();
    if(param != null)
    {
      timer = param.value - 0.0001f;

      //Debug.Log("force timeout on " + timerName + " : " + timer);
    }
  }

  public void stop()
  {
    timer = -1f;
  }

  public bool isRunning()
  {
    if (isFreezed()) return false;
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

    //Debug.Log("timer " + timerName + " updateEnd | ? " + timer + " / " + getProgress(), gameObject);

    if (!liveOnEnd) return;

    updateTimer();
  }

  protected void updateTimer() {

    //Debug.Log(timerName + " update "+isRunning());

    if (!isRunning()) return;
    
    //if(balancing != null && balancing.getCurrentParam() != null) Debug.Log(name + " | " + timer + " / " + balancing.getCurrentParam().value);
    
    TimerParams param = null;
    if (balancing != null) param = balancing.getCurrentParam();

    float mul = 1f;
    if (Input.GetKey(KeyCode.P)) mul = 10f;

    //chrono
    if (param == null)
    {
      timer += Time.deltaTime * mul;
      return;
    }

    //timer
    if (timer < param.value)
    {
      timer += Time.deltaTime * mul;

      //Debug.Log(transform.name+" | "+ timerName+" | "+timer +" / "+param.value+" (+"+Time.deltaTime+")");

      if (timer > param.value)
      {
        solveTimeout();
      }
    }

  }

  public bool hasBalancing() { return balancing != null; }
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
    if (balancing == null) return 0f;

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

#if UNITY_EDITOR
  private void OnDrawGizmos()
  {

    Handles.Label(transform.position + Vector3.right, ""+getTime());

  }
#endif

  public string toString()
  {
    string ct = "";

    ct += "\n(mul timer) " + name + " | " + timerName;
    ct += "\n  liveOnEnd ? " + liveOnEnd + " , liveOnStartup ? " + liveOnStartup;
    ct += "\n  active ? " + isActive() + " , freeze ? " + isFreezed() + " , progress ? " + getProgress();
    if (hasBalancing()) ct += "\n  " + getTime() + " / " + getTargetStep();
    else ct += "\n  no balancing";

    return ct;
  }

}
