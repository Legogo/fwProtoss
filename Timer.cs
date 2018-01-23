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

public class Timer : ArenaObject {

  public string timerName = "";

  protected DataTimer balancing;
  
  public bool liveOnStartup = true;
  public bool restartOnTimeout = true;
  public bool liveOnEnd = false;
  public bool timeoutOnPlay = false;

  public Action timeout;

  [Header("read only")]
  public float timer = -1f;

  TimerParam param;

  public void setupBalancing(DataTimer bal)
  {
    balancing = bal;

    param = balancing.fetchTimeParam();
    if (param.value == 0f)
    {
      Debug.LogError("param.value must not be 0f (oh shi- related) on " + balancing.name, balancing);
      Debug.LogError("timeMark : " + param.timeMark);
    }
    //if(balancing == null) Debug.LogWarning(timerName + " setupBalancing(null)", gameObject);
  }

  public void setupAndStart(DataTimer balancing = null)
  {
    //if (balancing == null) Debug.Log("balancing is null ?");

    //Debug.Log(name + " | " + timerName + " | balancing setup : " + balancing);
    setupBalancing(balancing);

    startTimer();
  }

  virtual public void startTimer()
  {

#if UNITY_EDITOR
    //if (balancing != null) Debug.Log("timer " + timerName + " starting as timeout. Timeout in " + balancing.fetchTimeParam().value, gameObject);
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

    //Debug.Log(name+" | "+timerName+ " | startTimer(), has balancing ? "+hasBalancing());
  }

  public void setupForTimeout()
  {
    if (balancing == null) return;

    param = balancing.fetchTimeParam();
    timer = param.value - 0.2f;
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

    //Debug.Log("<b>"+timerName+"</b> live "+timeStamp);

    //next param time !
    if(balancing != null && balancing.hasMultipleParams())
    {
      TimerParam param = balancing.fetchTimeParam();

      if (timeStamp > param.timeMark)
      {
        Debug.Log("<b>"+timerName+ "</b> next param " + timeStamp + " > " + param.timeMark);
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

  protected void updateTimer() {

    //if (timerName.Contains("spawn")) Debug.Log("<b>" + timerName + "</b> update | isRunning ? " + isRunning(), gameObject);
    
    if (!isRunning()) return;
    
    //if(balancing != null && balancing.getCurrentParam() != null) Debug.Log(name + " | " + timer + " / " + balancing.getCurrentParam().value);
    
    if (balancing != null) param = balancing.fetchTimeParam();

    float mul = 1f;
    if (Input.GetKey(KeyCode.P)) mul = 10f;

    //chrono
    if (!balancing.hasParam())
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

    param = balancing.fetchTimeParam();
    return param.value;
  }

  public float getProgress()
  {
    if (balancing == null) return 0f; //pas encore démarré

    param = balancing.fetchTimeParam();

    if (param.value == 0f)
    {
      Debug.LogError("param of timer '"+timerName+"' value == 0f ? oh shi- coming", gameObject);
      Debug.LogError("  balancing : " + balancing, balancing);
      return -1f;
    }

    return timer / param.value;
  }

  public void solveTimeout()
  {
    //if (balancing == null) Debug.LogError(name + " no balancing ? ", gameObject);

    //si le timer a jamais été call et qu'on arrive a la fin d'un round
    if (balancing == null) return;

    param = balancing.fetchTimeParam();
    
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
  
  override public string toString()
  {
    string ct = base.toString();

    //general info
    ct += "\n(mul timer) " + name + " | " + timerName;

    //chrono or timer ?
    if (hasBalancing()) ct += "\nsetup as timer (balancing : "+balancing+")";
    else ct += "\nsetup as chrono";

    //live ?
    if (!isRunning()) ct += "\nNOT RUNNING";
    else ct += "\ntimer : " + timer;

    //freeze stuff ?
    ct += "\n  liveOnEnd ? " + liveOnEnd + " , liveOnStartup ? " + liveOnStartup;
    ct += "\n  active ? " + isActive() + " , freeze ? " + isFreezed() + " , progress ? " + getProgress();

    if (_arena != null) ct += "\n  arena live ? " + _arena.isArenaStateLive() + " , end ? " + _arena.isArenaStateEnd();
    else ct += "\n  no arena ?";

    ct += "\n  eos idx : "+EngineObject.eos.IndexOf(this);

    //balancing stuff
    if (hasBalancing())
    {
      ct += "\n  getTime() " + getTime() + " / getTargetStep() " + getTargetStep();

      if (balancing == null) ct += "\nNO BALANCING";
      else
      {
        param = balancing.fetchTimeParam();
        ct += "\n" + timer + " / " + param.value;
      }

    }
    else ct += "\n  no balancing (Chrono ?)";

    return ct;
  }

}
