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
  
  protected override void onPlay()
  {
    base.onPlay();

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

  protected override void onStop()
  {
    base.onStop();
    loopedOnce = false;
  }

  protected override void onEnd()
  {
    base.onEnd();
    
    if (!loopedOnce && onVideoLoopFirstTime != null) onVideoLoopFirstTime();

    if (!loopedOnce) loopedOnce = true;

    //play(loopAtFrame);
    int idx = getCurrentClipIndex();
    setAtFrame(loopAtFrame[idx]);
  }
  
}
