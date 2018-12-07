using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// at the end of the video it will be asked to loop at a specific frame
/// </summary>

public class VideoPlayerLoop : VideoPlayerController {

  public int[] loopAtFrame;

  protected bool loopedOnce = false;

  public Action onVideoLoopFirstTime;

  protected override void setup()
  {
    base.setup();

    if (loopAtFrame.Length <= 0)
    {
      Debug.LogWarning("loop player but no loop frame content ??");
    }

  }

  protected override void eventPlay()
  {
    base.eventPlay();

    for (int i = 0; i < loopAtFrame.Length; i++)
    {
      VideoClip clip = getClip(i);
      if (loopAtFrame[i] > (int)clip.frameCount)
      {
        Debug.LogError("clip of index "+i+" asking to loop at frame " + loopAtFrame[i] + " but clip has " + clip.frameCount + " frames");
        Debug.Log(clip.name);
      }
    }
    
  }

  protected override void eventStop()
  {
    base.eventStop();
    loopedOnce = false;
  }

  protected override void eventEnd()
  {
    base.eventEnd();

    if (loopAtFrame.Length <= 0) return;

    if (!loopedOnce && onVideoLoopFirstTime != null) onVideoLoopFirstTime();

    if (!loopedOnce) loopedOnce = true;

    //play(loopAtFrame);
    int idx = getCurrentClipIndex();
    setAtFrame(loopAtFrame[idx]);
  }
  
}
