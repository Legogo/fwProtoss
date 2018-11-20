using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : EngineObject {

  public enum VideoState { IDLE, PLAY, STOP, END };

  public VideoClip clip;

  protected MeshRenderer canvas;
  protected VideoPlayer _vp;
  protected VideoState _state;

  public bool debugLogs = false;

  protected override void build()
  {
    base.build();
    _vp = GetComponent<VideoPlayer>();
    if(clip != null) _vp.clip = clip;
  }

  protected override void setup()
  {
    base.setup();
    visibility.hide();
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
          onPlay();
        }

        break;
      case VideoState.PLAY:
        
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

    Debug.Log(_vp.clip.name + " | end | loop ? "+_vp.isLooping);
  }
  
}
