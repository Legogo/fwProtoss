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

  protected long previousFrame = 0;

  public bool debugLogs = false;

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
    _state = VideoState.IDLE;

    videoPlayer.frame = 0; // at start
    videoPlayer.Play();

    canvas.enabled = false;

    if(debugLogs) Debug.Log("video controller is now playing : " + videoPlayer.clip.name);

    //onPlay();
  }

  public void stop()
  {
    videoPlayer.Stop();

    canvas.enabled = false;
  }

  public void play(int startAtFrame = 0)
  {
    _state = VideoState.IDLE;
    if (startAtFrame > 0) videoPlayer.frame = startAtFrame;
    videoPlayer.Play();
    //onPlay();
  }

  public override void updateEngine()
  {
    base.updateEngine();
    
    switch (_state)
    {
      case VideoState.IDLE:

        //Debug.Log(_vp.isPrepared+" , "+_vp.isPlaying);

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

        //sometimes the player stay at the same video frame for multiple engine frame
        if (previousFrame != videoPlayer.frame)
        {

          if (videoPlayer.frame > 2 && !canvas.enabled)
          {
            canvas.enabled = true;
          }

          if (frameSubs != null)
          {
            foreach (KeyValuePair<int, Action<int>> kp in frameSubs)
            {
              //Debug.Log(Time.frameCount + " , " + kp.Key + " ? " + _vp.frame);

              if (videoPlayer.frame == kp.Key)
              {
                kp.Value((int)videoPlayer.frame);
              }
            }
          }

          previousFrame = videoPlayer.frame;
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
  
  virtual protected void onResume()
  {
    _state = VideoState.PLAY;
    if(debugLogs) Debug.Log(videoPlayer.clip.name + " | resume | at frame : "+videoPlayer.frame);
    
  }
  virtual protected void onPlay()
  {
    _state = VideoState.PLAY;
    if (debugLogs) Debug.Log(videoPlayer.clip.name+" | play | at frame : "+videoPlayer.frame+" | total frames : "+videoPlayer.frameCount);

    visibility.show();
  }
  virtual protected void onStop()
  {
    _state = VideoState.STOP;
    if (debugLogs) Debug.Log(videoPlayer.clip.name + " | stop");
  }
  
  virtual protected void onEnd()
  {
    if(!videoPlayer.isLooping)
    {
      _state = VideoState.END;

      stop(); // not visible
    }

    if(debugLogs) Debug.Log(videoPlayer.clip.name + " | end | loop ? "+videoPlayer.isLooping);

    if (onVideoEnd != null) onVideoEnd();
  }
  
}
