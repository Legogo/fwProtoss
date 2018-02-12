using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Timer : ArenaObject {

  public string timerName = "";

  public bool liveOnStartup = true;
  public bool restartOnTimeout = true;
  public bool liveOnEnd = false;
  public bool timeoutOnPlay = false;

  [Header("read only")]
  public float timer = -1f;
  public float timerTimeout = -1f;

  protected DataTimer balancingTimer;

  public Action timeout;

  virtual public void setupTimer(DataTimer data)
  {
    balancingTimer = data;
  }

  virtual public void setupAndStart(DataTimer data)
  {
    setupTimer(data);
    startTimer();
  }

  virtual public void startTimer()
  {
    //unfreeze, activate
    launch();

    if(balancingTimer != null)
    {
      timerTimeout = balancingTimer.fetchTime();
    }
    
    //Debug.Log("timer is at 0");
    timer = 0f;
  }

  protected override void updateArenaLive(float timeStamp)
  {
    base.updateArenaLive(timeStamp);
    
    float mul = 1f;
    if (Input.GetKey(KeyCode.P)) mul = 10f;

    timer += Time.deltaTime * mul;

    if (timerTimeout > -1f)
    {
      if(timer >= timerTimeout)
      {
        solveTimeout();
      }
    }
  }

  public float getTime() { return timer; }

  public bool hasBalancing() { return balancingTimer != null; }

  virtual public bool isChrono() { return balancingTimer == null; }
  
  virtual public float getTimeRemaining()
  {
    //Debug.Log(getTargetStep() + " * " + getProgress());
    //float max = getTargetStep();
    if (isChrono()) return timer;

    return timerTimeout - timer;
  }

  virtual public float getTimeoutTime()
  {
    return timerTimeout;
  }

  virtual public void solveTimeout()
  {
    //loop timer ?
    if (restartOnTimeout) timer = 0f;
    else timer = -1f;

    if (timeout != null) timeout();
  }

  public bool isRunning()
  {
    if (isFreezed()) return false;
    return timer >= 0f;
  }

#if UNITY_EDITOR
  private void OnDrawGizmos()
  {

    Handles.Label(transform.position + Vector3.right, "" + getTime());

  }
#endif

  override public string toString()
  {
    string ct = base.toString();

    //general info
    ct += "\n(mul timer) " + name + " | " + timerName;
    
    //live ?
    if (!isRunning()) ct += "\nNOT RUNNING";
    else ct += "\ntimer : " + timer;
    
    return ct;
  }

}
