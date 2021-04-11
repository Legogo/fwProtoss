using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// feed data then call "play()"
/// </summary>

namespace ui
{
  public class UiMovement : UiAnimation
  {
    public UiMotionData data;

    public void feed(UiMotionData newData)
    {
      data = newData;
    }

    public override void reset()
    {
      base.reset();
      rec.setPixelPosition(data.origin);
    }

    protected override void updateAnimationProcess()
    {
      rec.setPixelPosition(Vector2.Lerp(data.origin, data.destination, getProgress()));
    }
  }

  [Serializable]
  public struct UiMotionData
  {
    public Vector2 origin;
    public Vector2 destination;
  }
}