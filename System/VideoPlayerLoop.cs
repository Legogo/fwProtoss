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

  protected override void build()
  {
    base.build();

    if(!videoPlayer.isLooping)
    {
      Debug.LogWarning("videoplayer is not looping, loop frame won't work !");
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

  protected override void solveLooping()
  {
    //base.solveLooping();

    if (!loopedOnce)
    {
      Debug.Log(getStamp() + "loop player | looped once");
      if (onVideoLoopFirstTime != null) onVideoLoopFirstTime();
      loopedOnce = true;
    }

    int idx = getCurrentClipIndex();
    int frameTarget = loopAtFrame[idx];

    Debug.Log(getStamp() + "loop player | setting up player at frame : " + frameTarget);

    setAtFrame(frameTarget);
  }
  
}
