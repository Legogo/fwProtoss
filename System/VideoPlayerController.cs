using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : EngineObject {

  public enum VideoState { IDLE, PLAY, STOP, END };

  public VideoClip[] clips;

  protected MeshRenderer canvas;
  protected VideoPlayer _vp;
  protected VideoState _state;

  public Action onVideoEnd;

  public Dictionary<int, Action<int>> frameSubs;

  protected long previousFrame = 0;

  public bool debugLogs = false;

  protected override void build()
  {
    base.build();
    _vp = GetComponent<VideoPlayer>();
    if(clips != null && clips.Length > 9) _vp.clip = clips[0];

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
    _vp.clip = clips[index];
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
  }

  protected override VisibilityMode getVisibilityType()
  {
    return VisibilityMode.MESH;
  }

  public void play()
  {
    _state = VideoState.IDLE;

    _vp.frame = 0; // at start
    _vp.Play();

    canvas.enabled = false;
    //onPlay();
  }

  public void stop()
  {
    _vp.Stop();
  }

  public void play(int startAtFrame = 0)
  {
    _state = VideoState.IDLE;
    if (startAtFrame > 0) _vp.frame = startAtFrame;
    _vp.Play();
    //onPlay();
  }

  public override void updateEngine()
  {
    base.updateEngine();
    
    switch (_state)
    {
      case VideoState.IDLE:

        //Debug.Log(_vp.isPrepared+" , "+_vp.isPlaying);

        if(_vp.isPlaying)
        {
          canvas.enabled = true;
          onPlay();
        }

        break;
      case VideoState.PLAY:
        
        //sometimes the player stay at the same video frame for multiple engine frame
        if(previousFrame != _vp.frame)
        {
          if(frameSubs != null)
          {
            foreach (KeyValuePair<int, Action<int>> kp in frameSubs)
            {
              //Debug.Log(Time.frameCount + " , " + kp.Key + " ? " + _vp.frame);

              if (_vp.frame == kp.Key)
              {
                kp.Value((int)_vp.frame);
              }
            }
          }

          previousFrame = _vp.frame;
        }
        //Debug.Log(_vp.frame + " / " + _vp.frameCount);

        if ((int)_vp.frame >= (int)_vp.frameCount) // at last frame of video
        {
          onEnd();
        }
        else if (!_vp.isPlaying) // not at end of video and not playing
        {
          onStop();
        }
        
        break;
      case VideoState.STOP:
        if(_vp.isPlaying)
        {
          onResume();
        }
        break;
    }
    
  }
  
  virtual protected void onResume()
  {
    _state = VideoState.PLAY;
    if(debugLogs) Debug.Log(_vp.clip.name + " | resume | at frame : "+_vp.frame);
    
  }
  virtual protected void onPlay()
  {
    _state = VideoState.PLAY;
    if (debugLogs) Debug.Log(_vp.clip.name+" | play | at frame : "+_vp.frame+" | total frames : "+_vp.frameCount);

    visibility.show();
  }
  virtual protected void onStop()
  {
    _state = VideoState.STOP;
    if (debugLogs) Debug.Log(_vp.clip.name + " | stop");
  }
  
  virtual protected void onEnd()
  {
    if(!_vp.isLooping)
    {
      _state = VideoState.END;
    }

    if(debugLogs) Debug.Log(_vp.clip.name + " | end | loop ? "+_vp.isLooping);

    if (onVideoEnd != null) onVideoEnd();
  }
  
}
