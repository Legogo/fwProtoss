using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// rich timer logic
/// 
/// - loop (auto restart)
/// - settings (DataTimer)
/// 
/// </summary>

public class TimerSteps : Timer
{
  protected DataTimerSteps balancingSteps;

  TimerParam param;

  public void setupBalancing(DataTimer bal)
  {
    balancingTimer = bal;
    balancingSteps = bal as DataTimerSteps;

    if (balancingSteps != null)
    {
      param = balancingSteps.fetchTimeParam();
      if (param.value == 0f)
      {
        Debug.LogError("param.value must not be 0f (oh shi- related) on " + balancingSteps.name);
        Debug.LogError("timeMark : " + param.timeMark);
      }
    }

    //if(balancing == null) Debug.LogWarning(timerName + " setupBalancing(null)", gameObject);
  }

  override public void setupAndStart(DataTimer balancing = null)
  {
    //if (balancing == null) Debug.Log("balancing is null ?");

    //Debug.Log(name + " | " + timerName + " | balancing setup : " + balancing);
    setupBalancing(balancing);

    startTimer();
  }

  override public void startTimer()
  {

#if UNITY_EDITOR
    //if (balancing != null) Debug.Log("timer " + timerName + " starting as timeout. Timeout in " + balancing.fetchTimeParam().value, gameObject);
    //else Debug.Log("timer " + timerName + " starting as a chrono (no balancing / timeout)", gameObject);
#endif

    //unfreeze, activate
    launch();

    if (balancingSteps != null)
    {
      //update interal param
      balancingSteps.fetchTimeParam();
    }

    //Debug.Log("timer is at 0");
    timer = 0f;

    //Debug.Log(name+" | "+timerName+ " | startTimer(), has balancing ? "+hasBalancing());
  }

  public void setupForTimeout()
  {
    if (balancingSteps == null) return;

    param = balancingSteps.fetchTimeParam();
    timer = param.value - 0.2f;
  }

  public void stop()
  {
    timer = -1f;
  }

  protected override void updateArenaLive(float timeStamp)
  {
    base.updateArenaLive(timeStamp);

    //Debug.Log("<b>"+timerName+"</b> live "+timeStamp);

    //next param time !
    if (balancingSteps != null && balancingSteps.hasMultipleParams())
    {
      TimerParam param = balancingSteps.fetchTimeParam();

      if (timeStamp > param.timeMark)
      {
        Debug.Log("<b>" + timerName + "</b> next param " + timeStamp + " > " + param.timeMark);
      }
    }

    updateTimer();
  }

  protected override void updateArenaEnd()
  {
    base.updateArenaEnd();

    //Debug.Log("timer <b>" + timerName + "</b> updateEnd | ? " + timer + " / " + getProgress(), gameObject);

    if (!liveOnEnd) return;

    updateTimer();
  }

  protected void updateTimer()
  {

    //if (timerName.Contains("spawn")) Debug.Log("<b>" + timerName + "</b> update | isRunning ? " + isRunning(), gameObject);

    if (!isRunning()) return;

    //if(balancing != null && balancing.getCurrentParam() != null) Debug.Log(name + " | " + timer + " / " + balancing.getCurrentParam().value);

    if (balancingSteps != null) param = balancingSteps.fetchTimeParam();

    float mul = 1f;
    if (Input.GetKey(KeyCode.P)) mul = 10f;

    //chrono
    if (!balancingSteps.hasParam())
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

  override public bool isChrono() { return balancingSteps == null; }
  
  public override float getTimeoutTime()
  {
    if (balancingSteps == null) return 0f;

    param = balancingSteps.fetchTimeParam();
    return param.value;
  }

  public float getProgress()
  {
    if (balancingSteps == null) return 0f; //pas encore démarré

    param = balancingSteps.fetchTimeParam();

    if (param.value == 0f)
    {
      Debug.LogError("param of timer '" + timerName + "' value == 0f ? oh shi- coming", gameObject);
      Debug.LogError("  balancing : " + balancingSteps, balancingSteps);
      return -1f;
    }

    return timer / param.value;
  }

  override public void solveTimeout()
  {
    //if (balancing == null) Debug.LogError(name + " no balancing ? ", gameObject);

    //si le timer a jamais été call et qu'on arrive a la fin d'un round
    if (balancingSteps == null) return;

    param = balancingSteps.fetchTimeParam();

    //Debug.Log("<b>"+timerName + "</b> solving timeout !", gameObject);

    //loop timer ?
    if (restartOnTimeout) timer = 0f;
    else timer = -1f;

    if (timeout != null) timeout();
  }
  
  override public string toString()
  {
    string ct = base.toString();

    //general info
    ct += "\n(mul timer) " + name + " | " + timerName;

    //chrono or timer ?
    if (hasBalancing()) ct += "\nsetup as timer (balancing : " + balancingSteps + ")";
    else ct += "\nsetup as chrono";

    //live ?
    if (!isRunning()) ct += "\nNOT RUNNING";
    else ct += "\ntimer : " + timer;

    //freeze stuff ?
    ct += "\n  liveOnEnd ? " + liveOnEnd + " , liveOnStartup ? " + liveOnStartup;
    ct += "\n  freeze ? " + isFreezed() + " , progress ? " + getProgress();

    if (_arena != null) ct += "\n  arena live ? " + _arena.isArenaStateLive() + " , end ? " + _arena.isArenaStateEnd();
    else ct += "\n  no arena ?";

    //balancing stuff
    if (hasBalancing())
    {
      ct += "\n  getTime() " + getTime() + " / getTargetStep() " + getTimeoutTime();

      if (balancingSteps == null) ct += "\nNO BALANCING";
      else
      {
        param = balancingSteps.fetchTimeParam();
        ct += "\n" + timer + " / " + param.value;
      }

    }
    else ct += "\n  no balancing (Chrono ?)";

    return ct;
  }

}
