using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : EngineObject {

  public enum VideoState { IDLE, PLAY, STOP, SPAWN, PAUSED, END };

  public VideoClip[] clips;

  protected MeshRenderer meshCanvas;
  public VideoPlayer videoPlayer;
  protected VideoState _state;

  public bool skippable = false;
  public bool hideOnStop = false;
  public bool pauseAtEnd = false;

  public float scale = 1f;
  //public bool keepLastFrameVisible = true;

  public Action onPlay; // when the video started
  public Action onVideoEnd;

  public Dictionary<int, Action<int>> frameSubs;
  
  protected long frameHead = 0;
  protected long lastFrame = 0;

  protected override void build()
  {
    base.build();

    //Debug.Log("build");

    videoPlayer = GetComponent<VideoPlayer>();
    if (videoPlayer == null) Debug.LogError("no video player for " + name+" ?", transform);

    if(clips != null && clips.Length > 9) videoPlayer.clip = clips[0];

    meshCanvas = transform.GetComponentInChildren<MeshRenderer>();
    if (meshCanvas == null) Debug.LogError("no mesh canvas ?");
    else Debug.Log(meshCanvas);

    videoPlayer.targetMaterialRenderer = meshCanvas;

    Vector3 lScale =  meshCanvas.transform.localScale;
    lScale *= scale;
    meshCanvas.transform.localScale = lScale;

    //videoPlayer.loopPointReached += onLoop;
    //meshCanvas = HalperComponentsGenerics.getComponentContext<MeshRenderer>(transform, "canvas");
  }

  virtual protected void onLoop(VideoPlayer vp)
  {
    Debug.Log("onLoop");

    frameHead = 0;
    lastFrame = 0;

  }

  protected override void setup()
  {
    base.setup();
    visibility.hide();
  }
  
  public void setupClipAndPlay(int index)
  {
    //stop();
    videoPlayer.clip = clips[index];

    Debug.Log(getStamp()+"swapped video (index : "+ index+") for " + videoPlayer.clip.name, transform);

    play();
  }

  public int getNextClipIndex()
  {
    int idx = getCurrentClipIndex() + 1;
    if (idx > clips.Length - 1) idx = 0;
    return idx;
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
    play(); // at frame 0
  }
  
  public void play(int startAtFrame = 0)
  {
    _state = VideoState.IDLE;

    if (!videoPlayer.isPlaying)
    {
      Debug.Log("video player play");
      videoPlayer.Play();
    }

    videoPlayer.frame = startAtFrame;
    frameHead = startAtFrame;
    lastFrame = startAtFrame;
    
    //videoPlayer.playbackSpeed = 1f;

    meshCanvas.enabled = false;

    Debug.Log(getStamp() + "video controller is now playing : " + videoPlayer.clip.name+" , head = "+frameHead, transform);
    Debug.Log("  L " + videoPlayer.clip.name + " " + videoPlayer.frame + " / " + videoPlayer.frameCount);
    Debug.Log("  L head : " + frameHead + " | " + lastFrame);

  }

  public void setAtFrame(int newFrame)
  {
    if (!videoPlayer.isPlaying)
    {
      Debug.Log("video player play");
      videoPlayer.Play();
    }

    videoPlayer.frame = newFrame;
    frameHead = newFrame;
    lastFrame = newFrame;

    Debug.Log(getStamp() + "setAtFrame(" + newFrame + ") | videplayer frame " + videoPlayer.frame + " / " + videoPlayer.frameCount);
    Debug.Log("  L head : " + frameHead);
    Debug.Log("  L last : " + lastFrame);

    //player will take a frame to change frame to new target frame ?
    //_state = VideoState.IDLE;
    _state = VideoState.SPAWN;
  }

  public void stop()
  {
    _state = VideoState.STOP;
    
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

    checkCanvasVisibility();
    solveSkippable();

    if (videoPlayer.isPlaying && videoPlayer.isPrepared)

    //return;

    switch (_state)
    {
      case VideoState.IDLE:
        
        //on att que le vp commence a jouer pour balancer l'event
        if(videoPlayer.isPlaying && videoPlayer.frame > 1)
        {
          Debug.Log(getStamp() + videoPlayer.clip.name + " has started playing, frame count ("+videoPlayer.frame+") is positive, calling eventPlay()");
          eventPlay();
        }

        break;
      case VideoState.PLAY:

        headCatchup();

        checkForEndOfVideo();

        break;
      case VideoState.STOP:
        if(videoPlayer.isPlaying)
        {
          Debug.Log(getStamp() + "resume from stop");
          eventResume();
        }
        break;
      case VideoState.PAUSED:
        if (videoPlayer.isPlaying)
        {
          Debug.Log(getStamp() + "resume from pause");
          eventResume();
        }
        break;
      case VideoState.SPAWN:
        if(videoPlayer.frame >= frameHead)
        {
          Debug.Log(getStamp() + "is now ready after spawning");
          _state = VideoState.PLAY;
        }
        break;
    }

  }

  protected void checkForEndOfVideo()
  {
    bool atEnd = false;

    if (videoPlayer.frame >= (int)videoPlayer.frameCount)
    {
      Debug.Log("go to last frame of current clip");
      atEnd = true;
    }

    if(!atEnd)
    {
      //sometimes (on ios) the player don't get to the very last frame and jump right away to the beginning of the video (loop case)
      //component just reseted it's frame count (loop ?)
      int bufferLength = 4; // how many frames the player might have skipped when arrived at the end of the video
      int last = (int)videoPlayer.frameCount - bufferLength;
      if (lastFrame >= last && videoPlayer.frame < lastFrame)
      {
        Debug.Log("videoplayer is playing a frame " + videoPlayer.frame + " that is before last frame : " + lastFrame);
        atEnd = true;
      }
    }

    if (atEnd)
    {
      Debug.Log(frameHead + " | " + videoPlayer.frame + "/" + videoPlayer.frameCount);
      eventEnd();
    }

    lastFrame = videoPlayer.frame;
  }

  protected void headCatchup()
  {
    //int diff = (int)frameHead - (int)videoPlayer.frame;
    //if(diff > 1) Debug.Log("head progression delta " + frameHead + " VS " + videoPlayer.frame + " = " + diff);
    
    //catchup
    //sometimes the player stay at the same video frame for multiple engine frame
    while (frameHead < videoPlayer.frame)
    {
      checkFrame();
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
          Debug.Log("  L callback for time "+kp.Key);

          kp.Value((int)frameHead);
        }
      }

    }

    frameHead++;
  }

  protected void checkCanvasVisibility()
  {
    if (_state != VideoState.PLAY) return;

    //display canvas if it was hidden during for loading
    if (videoPlayer.frame > 2 && !meshCanvas.enabled)
    {
      meshCanvas.enabled = true;
    }

  }

  protected void solveSkippable()
  {
    if (_state != VideoState.PLAY) return;

    if (skippable)
    {
      if (Input.GetMouseButtonUp(0))
      {
        Debug.Log(getStamp() + "skipped video : " + videoPlayer.clip.name);
        videoPlayer.frame = (long)videoPlayer.clip.frameCount - 3;
        return;
      }
    }

  }


  protected VideoClip getClip(int idx)
  {
    return clips[idx];
  }
  
  public int getCurrentClipIndex()
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

    log("event play " + videoPlayer.clip.name+"("+videoPlayer.frameCount+")");

    if (onPlay != null) onPlay();
  }

  virtual protected void eventStop()
  {
    _state = VideoState.STOP;
    Debug.Log(getStamp() + videoPlayer.clip.name + " | eventStop", transform);

    log("event stop");
  }

  virtual protected void solveLooping()
  {
    setAtFrame(0);
  }

  protected void eventEnd()
  {
    _state = VideoState.END;

    Debug.Log(getStamp() + videoPlayer.clip.name + " | eventEnd", transform);

    if (videoPlayer.isLooping)
    {
      Debug.Log(getStamp()+"event end > looping");
      solveLooping();
    }
    else if (pauseAtEnd)
    {
      Debug.Log(getStamp() + videoPlayer.clip.name + " | eventEnd | paused");

      videoPlayer.Pause();

      log("end > pauseatend (player is playing ? "+videoPlayer.isPlaying);

      _state = VideoState.PAUSED;
    }
    else
    {
      Debug.Log(getStamp() + "player not looping, calling stop");
      stop(); // not visible
    }

    //callback on video end
    if (onVideoEnd != null) onVideoEnd();
  }

  protected override string getStamp()
  {
    //return base.getStamp();
    return "<color=yellow>fwp VideoPlayer</color> | ";
  }

  public override string toString()
  {
    string ct = base.toString();

    ct = "[video controller]";
    ct += "\n  L state : " + _state;
    ct += "\n  L component playing : " + videoPlayer.isPlaying;
    ct += "\n  L frame : " + videoPlayer.frame + " / " + videoPlayer.frameCount;
    ct += "\n  L head : " + frameHead;
    
    return ct;
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
