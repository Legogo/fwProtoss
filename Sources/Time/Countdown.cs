using System;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : TimeObject
{
  public float timeoutTime = 1f; // -1 is disabled

  public Action onTimeout;

  public override void play(bool resetTime = true)
  {
    base.play(resetTime);

    if (balancingTimer != null)
    {
      timeoutTime = balancingTimer.getRandomTimeTarget();
    }
  }

  public override void update()
  {
    float current = getTime(); // keep to create event

    base.update(); // inject delta

    if (timeoutTime < 0f) return;

    //new time this frame ?
    if (current < timeoutTime && getTime() >= timeoutTime)
    {
      solveTimeout();
    }

  }

  virtual protected void solveTimeout()
  {
    if(onTimeout != null) onTimeout();
  }

  public float getTimeoutTime() { return timeoutTime; }

  public override float getTimeRemaining()
  {
    return Mathf.Max(0f, timeoutTime - getTime());
  }
}
