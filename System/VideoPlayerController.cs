using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : EngineObject {

  public enum VideoState { IDLE, PLAY, STOP, END };

  public VideoClip[] clips;

  protected MeshRenderer meshCanvas;
  public VideoPlayer videoPlayer;
  protected VideoState _state;

  public bool skippable = false;
  public bool hideOnStop = false;
  //public bool keepLastFrameVisible = true;

  public Action onPlay; // when the video started
  public Action onVideoEnd;

  public Dictionary<int, Action<int>> frameSubs;

  protected long frameHead = 0;
  
  protected override void build()
  {
    base.build();

    videoPlayer = GetComponent<VideoPlayer>();
    if(clips != null && clips.Length > 9) videoPlayer.clip = clips[0];

    meshCanvas = HalperComponentsGenerics.getComponentContext<MeshRenderer>(transform, "canvas");
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

  public void playDefaultClip()
  {
    videoPlayer.clip = clips[0];
    play();
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

    videoPlayer.playbackSpeed = 1f;

    meshCanvas.enabled = false;

    Debug.Log("video controller is now playing : " + videoPlayer.clip.name+" , head = "+frameHead);

    //onPlay();
  }

  public void setAtFrame(int newFrame)
  {
    if (!videoPlayer.isPlaying) videoPlayer.Play();

    videoPlayer.frame = newFrame;
    frameHead = newFrame;
    _state = VideoState.IDLE;
  }

  public void stop()
  {
    if(videoPlayer == null)
    {
      Debug.LogWarning("no video player ?");
      return;
    }

    /*
    if (keepLastFrameVisible)
    {
      setAtFrame((int)videoPlayer.clip.frameCount);
      
      videoPlayer.playbackSpeed = 0f;

      videoPlayer.frame = (long)videoPlayer.clip.frameCount;
    }
    */

    if (videoPlayer != null) videoPlayer.Stop();

    if (hideOnStop)
    {
      if (meshCanvas != null) meshCanvas.enabled = false;
    }

  }

  public override void updateEngine()
  {
    base.updateEngine();
    
    switch (_state)
    {
      case VideoState.IDLE:
        
        //on att que le vp commence a jouer pour balancer l'event
        if(videoPlayer.isPlaying && videoPlayer.frame > 1)
        {
          eventPlay();
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

        if (videoPlayer.frame > 2 && !meshCanvas.enabled)
        {
          meshCanvas.enabled = true;
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

        //Debug.Log(videoPlayer.frame + " / " + videoPlayer.frameCount);

        if ((int)videoPlayer.frame >= (int)videoPlayer.frameCount) // at last frame of video
        {
          eventEnd();
        }
        else if (!videoPlayer.isPlaying) // not at end of video and not playing
        {
          eventStop();
        }
        
        break;
      case VideoState.STOP:
        if(videoPlayer.isPlaying)
        {
          eventResume();
        }
        break;
    }
    
  }

  protected void checkFrame()
  {
    //Debug.Log("checking callbacks for frame : " + frameHead);
    if (frameSubs != null)
    {

      foreach (KeyValuePair<int, Action<int>> kp in frameSubs)
      {
        if (frameHead == kp.Key)
        {
          //Debug.Log("  L callback !");

          kp.Value((int)frameHead);
        }
      }

    }

    frameHead++;
  }


  protected VideoClip getClip(int idx)
  {
    return clips[idx];
  }
  
  protected int getCurrentClipIndex()
  {
    for (int i = 0; i < clips.Length; i++)
    {
      if (clips[i] == videoPlayer.clip) return i;
    }
    return -1;
  }

  virtual protected void eventResume()
  {
    _state = VideoState.PLAY;
    Debug.Log(videoPlayer.clip.name + " | <b>eventResume</b> | at frame : " + videoPlayer.frame);
  }
  virtual protected void eventPlay()
  {
    _state = VideoState.PLAY;
    Debug.Log(videoPlayer.clip.name+ " | <b>eventPlay</b> | at frame : " + videoPlayer.frame+" | total frames : "+videoPlayer.frameCount);

    visibility.show();

    if (onPlay != null) onPlay();
  }
  virtual protected void eventStop()
  {
    _state = VideoState.STOP;
    Debug.Log(videoPlayer.clip.name + " | eventStop");
  }
  
  virtual protected void eventEnd()
  {
    if(!videoPlayer.isLooping)
    {
      _state = VideoState.END;
      
      stop(); // not visible
    }

    Debug.Log(videoPlayer.clip.name + " | eventEnd | loop ? " + videoPlayer.isLooping);
    frameHead = 0;

    if (onVideoEnd != null) onVideoEnd();
  }
  
}
