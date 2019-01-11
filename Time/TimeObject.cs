using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class TimeObject
{
  protected DataTimer balancingTimer;
  bool running = false;

  float time = 0f;

  virtual public void setupBalancing(DataTimer data)
  {
    balancingTimer = data;
  }
  
  virtual public void reset()
  {
    time = 0f;
  }

  virtual public void play(bool resetTime = true)
  {
    running = true;

    if (resetTime) reset();
  }

  virtual public void pause()
  {
    running = false;
  }

  virtual public void stop()
  {
    pause();
    reset();
  }

  virtual public void update()
  {
    if (!isRunning()) return;

    float mul = 1f;

#if debug
    //debug
    if (Input.GetKey(KeyCode.P)) mul = 10f;
#endif

    time += Time.deltaTime * mul;
  }

  public bool hasBalancing() { return balancingTimer != null; }

  //in seconds
  public float getTime() { return time; }

  public bool isRunning() { return running; }

  abstract public float getTimeRemaining();
  
}
