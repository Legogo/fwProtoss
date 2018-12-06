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

  public float scale = 1f;
  //public bool keepLastFrameVisible = true;

  public Action onPlay; // when the video started
  public Action onVideoEnd;

  public Dictionary<int, Action<int>> frameSubs;

  protected long frameHead = 0;
  
  protected override void build()
  {
    base.build();

    videoPlayer = GetComponent<VideoPlayer>();
    if (videoPlayer == null) Debug.LogError("no video player for " + name+" ?", transform);

    if(clips != null && clips.Length > 9) videoPlayer.clip = clips[0];

    meshCanvas = transform.GetComponentInChildren<MeshRenderer>();
    videoPlayer.targetMaterialRenderer = meshCanvas;

    Vector3 lScale =  meshCanvas.transform.localScale;
    lScale *= scale;
    meshCanvas.transform.localScale = lScale;

    //meshCanvas = HalperComponentsGenerics.getComponentContext<MeshRenderer>(transform, "canvas");
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

    Debug.Log(getStamp()+"swapped video (index : "+ index+") for " + videoPlayer.clip.name, transform);

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

    Debug.Log(getStamp()+"    subscribed at frame " + frame+" on video player : "+name, transform);
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

    Debug.Log(getStamp() + "video controller is now playing : " + videoPlayer.clip.name+" , head = "+frameHead, transform);

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
      Debug.LogWarning(getStamp() + "no video player ?");
      return;
    }
    
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
          Debug.Log(getStamp() + videoPlayer.clip.name + " has started playing, frame count is positive, calling eventPlay()");
          eventPlay();
        }

        break;
      case VideoState.PLAY:

        if (!videoPlayer.isPlaying) // not at end of video and not playing
        {
          Debug.Log(getStamp() + " videoplayer is not playing and state is PLAY, calling eventStop()");
          eventStop();
          return;
        }

        if (skippable)
        {
          if (Input.GetMouseButtonUp(0))
          {
            Debug.Log(getStamp() + "skipped video : " + videoPlayer.clip.name);
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
    Debug.Log(getStamp() + videoPlayer.clip.name + " | <b>eventResume</b> | at frame : " + videoPlayer.frame, transform);
  }
  virtual protected void eventPlay()
  {
    _state = VideoState.PLAY;
    Debug.Log(getStamp() + videoPlayer.clip.name+ " | <b>eventPlay</b> | at frame : " + videoPlayer.frame+" | total frames : "+videoPlayer.frameCount, transform);

    visibility.show();

    if (onPlay != null) onPlay();
  }
  virtual protected void eventStop()
  {
    _state = VideoState.STOP;
    Debug.Log(getStamp() + videoPlayer.clip.name + " | eventStop", transform);
  }
  
  virtual protected void eventEnd()
  {
    if(!videoPlayer.isLooping)
    {
      _state = VideoState.END;
      
      stop(); // not visible
    }

    Debug.Log(getStamp() + videoPlayer.clip.name + " | eventEnd | loop ? " + videoPlayer.isLooping, transform);
    frameHead = 0;

    if (onVideoEnd != null) onVideoEnd();
  }
  
  static protected string getStamp()
  {
    return "<color=yellow>fwp VideoPlayer</color> | ";
  }

#if UNITY_EDITOR
  private void OnDrawGizmos()
  {
    if (Application.isPlaying) return;

    if(clips.Length > 0)
    {
      VideoPlayer video = GetComponent<VideoPlayer>();
      if(video != null)
      {
        video.playOnAwake = false;
        video.waitForFirstFrame = true;

        video.clip = clips[0];
        MeshRenderer msh = transform.GetChild(0).GetComponent<MeshRenderer>();
        video.targetMaterialRenderer = msh;

        //msh.sharedMaterial.mainTexture = video.clip.
      }
    }
  }
#endif
}
