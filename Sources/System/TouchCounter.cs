using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TouchCounter {

  float touchTimer = 0f;
  int count = 0;

  public Action onDoubleTap;
  public Action onTripleTap;
  
  public void reset()
  {
    count = 0;
    touchTimer = -1f;
  }

  public void event_touch()
  {
    if (touchTimer <= 0f)
    {
      touchTimer = 0.2f;
    }
    else if (touchTimer > 0f)
    {
      //Debug.Log("toggle pause");
      count++;
      touchTimer = 0f;
    }

    if(count == 1)
    {
      if (onDoubleTap != null) onDoubleTap();
    }
    else if(count == 2)
    {
      if (onTripleTap != null) onTripleTap();
    }
  }

  public void update()
  {

    if (touchTimer > 0f)
    {
      touchTimer -= Time.deltaTime;
      //Debug.Log(touchTimer);
    }
    else if(touchTimer < 0f)
    {
      count = 0;
      //Debug.Log("reset");
    }

  }
}
