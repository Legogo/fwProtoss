using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// at the end of the video it will be asked to loop at a specific frame
/// </summary>

public class VideoPlayerLoop : VideoPlayerController {

  public int loopAtFrame = 0;

  protected bool loopedOnce = false;

  public Action onVideoLoopFirstTime;
  
  protected override void onPlay()
  {
    base.onPlay();

    if (loopAtFrame > (int)_vp.frameCount)
    {
      Debug.LogError("asking to loop at frame " + loopAtFrame + " but clip has " + _vp.frameCount + " frames");
      Debug.Log(_vp.clip.name);
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

    play(loopAtFrame);
  }
  
}
