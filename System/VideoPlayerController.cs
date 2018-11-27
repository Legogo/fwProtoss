using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : EngineObject {

  public enum VideoState { IDLE, PLAY, STOP, END };

  public VideoClip[] clips;

  protected MeshRenderer canvas;
  public VideoPlayer videoPlayer;
  protected VideoState _state;

  public bool skippable = false;
  public Action onVideoEnd;

  public Dictionary<int, Action<int>> frameSubs;

  protected long frameHead = 0;
  
  protected override void build()
  {
    base.build();
    videoPlayer = GetComponent<VideoPlayer>();
    if(clips != null && clips.Length > 9) videoPlayer.clip = clips[0];

    canvas = HalperComponentsGenerics.getComponentContext<MeshRenderer>(transform, "canvas");
  }

  protected override void setup()
  {
    base.setup();
    visibility.hide();
  }

  public void setupClipAndPlay(int index)
  {
    stop();
    videoPlayer.clip = clips[index];

    Debug.Log("swapped video for " + videoPlayer.clip.name);

    play();
  }

  public void subscribeAtFrame(int frame, Action<int> callback)
  {
    if (frameSubs == null) frameSubs = new Dictionary<int, Action<int>>();

    if (!frameSubs.ContainsKey(frame))
    {
      frameSubs.Add(frame, null);
    }

    frameSubs[frame] += callback;

    Debug.Log("    subscribed at frame " + frame + " in video " + videoPlayer.clip.name);
  }

  protected override VisibilityMode getVisibilityType()
  {
    return VisibilityMode.MESH;
  }

  public void play()
  {
    play(0);
    
    //onPlay();
  }

  public void play(int startAtFrame = 0)
  {
    _state = VideoState.IDLE;

    frameHead = startAtFrame;

    videoPlayer.frame = frameHead;
    if(!videoPlayer.isPlaying) videoPlayer.Play();
    
    canvas.enabled = false;

    Debug.Log("video controller is now playing : " + videoPlayer.clip.name+" , head = "+frameHead);

    //onPlay();
  }

  public void setAtFrame(int newFrame)
  {
    videoPlayer.frame = newFrame;
    frameHead = newFrame;
    _state = VideoState.IDLE;
  }

  public void stop()
  {
    videoPlayer.Stop();

    canvas.enabled = false;
  }

  public override void updateEngine()
  {
    base.updateEngine();
    
    switch (_state)
    {
      case VideoState.IDLE:
        
        //on att que le vp commence a jouer pour balancer l'event
        if(videoPlayer.isPlaying)
        {
          onPlay();
        }

        break;
      case VideoState.PLAY:

        if (skippable)
        {
          if (Input.GetMouseButtonUp(0))
          {
            Debug.Log("skipped video : " + videoPlayer.clip.name);
            videoPlayer.frame = (long)videoPlayer.clip.frameCount - 3;
          }
        }

        if (videoPlayer.frame > 2 && !canvas.enabled)
        {
          canvas.enabled = true;
        }

        //le video player met plusieurs frame a bien se resynchro
        if (frameHead > videoPlayer.frame) return;

        /*
        if(frameHead > videoPlayer.frame)
        {
          Debug.LogWarning("frame head is after current video player frame ???");
          Debug.Log("  L clip ? "+videoPlayer.clip.name + " | playing ? " + videoPlayer.isPlaying);
          Debug.Log("  L state ? "+_state+" | head ? "+frameHead+" / vp frame ? "+ videoPlayer.frame, videoPlayer.transform);
          return;
        }
        */

        //sometimes the player stay at the same video frame for multiple engine frame
        while (frameHead < videoPlayer.frame)
        {
          checkFrame();
        }
        
        //Debug.Log(_vp.frame + " / " + _vp.frameCount);

        if ((int)videoPlayer.frame >= (int)videoPlayer.frameCount) // at last frame of video
        {
          onEnd();
        }
        else if (!videoPlayer.isPlaying) // not at end of video and not playing
        {
          onStop();
        }
        
        break;
      case VideoState.STOP:
        if(videoPlayer.isPlaying)
        {
          onResume();
        }
        break;
    }
    
  }

  protected void checkFrame()
  {
    //Debug.Log("checking callbacks for frame : " + frameHead);

    foreach (KeyValuePair<int, Action<int>> kp in frameSubs)
    {
      if (frameHead == kp.Key)
      {
        //Debug.Log("  L callback !");

        kp.Value((int)frameHead);
      }
    }

    frameHead++;
  }
  
  virtual protected void onResume()
  {
    _state = VideoState.PLAY;
    Debug.Log(videoPlayer.clip.name + " | resume | at frame : "+videoPlayer.frame);
    
  }
  virtual protected void onPlay()
  {
    _state = VideoState.PLAY;
    Debug.Log(videoPlayer.clip.name+" | <b>onPlay</b> | at frame : "+videoPlayer.frame+" | total frames : "+videoPlayer.frameCount);

    visibility.show();
  }
  virtual protected void onStop()
  {
    _state = VideoState.STOP;
    Debug.Log(videoPlayer.clip.name + " | stop");
  }
  
  virtual protected void onEnd()
  {
    if(!videoPlayer.isLooping)
    {
      _state = VideoState.END;

      stop(); // not visible
    }

    Debug.Log(videoPlayer.clip.name + " | end | loop ? "+videoPlayer.isLooping);
    frameHead = 0;

    if (onVideoEnd != null) onVideoEnd();
  }
  
}
