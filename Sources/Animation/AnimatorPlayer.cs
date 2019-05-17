using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// to use animator player you need to have a "speed" parameter on each state that can multiply speed of animation reading
/// </summary>

public class AnimatorPlayer
{
  public const string noneState = "none";

  protected Transform owner;
  protected Animator animator;

  protected AnimatorStateInfo animInfo;
  protected AnimationClip state_clip;

  protected bool _paused = false;
  protected bool _playing = false;
  protected int layer_active_index = 0;
  
  private float animStartTimeRatio = 0f; // [0,1]

  //ms
  private float animTimeLength = 0f;
  private float animCurrentTime = 0f;
  private float animTimePrevious = 0f;

  private string animStateName = "";
  private float animSpeed = 1f;

  public event Action onAnimEnd;
  
  public AnimatorPlayer(Transform parent)
  {
    owner = parent;
    
    //trop tot (prop copy)
    //animator = owner.animator;

    //at this point owner.animator doit avoir solutionner les trucs de refs (prop copy)
    animator = owner.GetComponent<Animator>();
    if (animator == null) animator = owner.GetComponentInChildren<Animator>();

    if (animator == null)
    {
      Debug.LogError("this player should have an animator at this point", parent.gameObject);
    }
  }
  
  public AnimatorPlayer launch(string stateName, float startOffset = 0f)
  {
    stop();
    
    state_clip = getClipByName(stateName);
    
    if (state_clip != null)
    {
      animTimeLength = 1f * state_clip.length;
      animStartTimeRatio = startOffset / animTimeLength;

      _paused = false;
      _playing = true;
      animStateName = stateName;

      //Debug.Log("playing " + stateName);

      animator.Play(animStateName, layer_active_index, animStartTimeRatio);
    }
    else
    {
      Debug.LogWarning("no clip returned by state "+stateName);
    }

    return this;
  }

  /// <summary>
  /// must be called by parent to make the animation fo forward
  /// </summary>
  public void update()
  {
    if (_playing || _paused) return;

    animInfo = animator.GetCurrentAnimatorStateInfo(layer_active_index);
    animTimeLength = animInfo.length;
    
    float stepSpeed = GameTime.deltaTime * animSpeed;
    
    //Debug.Log("  current "+animCurrentTime+" / "+target+" ("+stepSpeed+")");
    animCurrentTime = Mathf.MoveTowards(animCurrentTime, animTimeLength, stepSpeed);
    
    float timeNorm = animCurrentTime / animTimeLength;
    //Debug.Log("  result " + animCurrentTime + " / " + animTimeLength + " = " +timeNorm);

    setAtTime(timeNorm);
  }

  /// <summary>
  /// updated by parent
  /// </summary>
  public void update_check()
  {
    if (isClipLooping()) return;

    animInfo = animator.GetCurrentAnimatorStateInfo(layer_active_index);

    //on launch
    //animTimeLength = animInfo.length;

    animCurrentTime = animInfo.normalizedTime * animTimeLength;
    //Debug.Log(">>time " + animCurrentTime + " , name " + animStateName);
    //Debug.Log("speed "+animInfo.speed + " , length " + animInfo.length);


    //float mod = animTimeLength % animCurrentTime;

    if (!isPlaying(animStateName))
    {
      //Debug.Log("done playing " + animStateName);
      stop();
      if (onAnimEnd != null) onAnimEnd();
    }
  }
  
  /// <summary>
  /// remet la timeline a 0
  /// </summary>
  public void reset()
  {
    animCurrentTime = animStartTimeRatio * animTimeLength;
    setAtTime(0f);
  }

  protected void forceAtTimeByRatio(float timeRatio)
  {
    setAtTime(timeRatio * animTimeLength);
  }

  public void setAtTime(float timeRatio)
  {
    animator.Play(animInfo.fullPathHash, -1, timeRatio);
    Debug.Log("set at time : "+animInfo.fullPathHash + " -> " + timeRatio);
  }

  public bool isClipLooping()
  {
    if (state_clip == null) return false;
    return state_clip.isLooping;
  }

  public bool checkAnimAtEnd(float endTime)
  {
    return Mathf.Approximately(Mathf.Clamp01(getTimeRatio()), endTime);
  }

  public bool isPlaying()
  {
    return _playing;
  }

  public bool isPlaying(string stateName)
  {
    if (!isPlaying()) return false;

    animInfo = animator.GetCurrentAnimatorStateInfo(layer_active_index);

    return animInfo.IsName(stateName);

    /*
    if (animStateName.Length <= 0)
    {
      //Debug.LogWarning("animator player is playing nothing");
      return false;
    }
    //Debug.Log(stateName + " , " + animStateName);
    return stateName == animStateName;
    */
  }
  
  /// <summary>
  /// freeze l'anim où elle est
  /// </summary>
  public void pause()
  {
    _paused = true;
    //Debug.Log(animStateName + " , " + _paused);
    setAnimatorSpeed(0f);
  }
  public void resume()
  {
    _paused = false;
    setAnimatorSpeed(animSpeed);
  }

  /// <summary>
  /// arrête l'animation et relache le controle de l'animator
  /// </summary>
  public void stop()
  {
    //animator.Play("none");
    animStateName = "";
    animTimeLength = -1f;
    _playing = false;
    _paused = false;
  }
  
  /* la layer qui a un normTime en cours ]0,1[ */
  protected int getActiveLayerIndex()
  {
    for (int i = 0; i < animator.layerCount; i++)
    {
      AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(i);
      //if (info.normalizedTime > 0f && info.normalizedTime < 1f)
      if (info.normalizedTime < 1f)
      {
        //Debug.Log("   found layer #"+i+" at time : " + info.normalizedTime+" ("+animator.gameObject.name+")", animator.gameObject);
        return i;
      }
    }

    return -1;
  }

  public float getTimeRatio() { return animInfo.normalizedTime; }

  public void setAnimationSpeed(float newSpeed)
  {
    animSpeed = newSpeed;
    setAnimatorSpeed(newSpeed);
  }

  protected void setAnimatorSpeed(float newSpeed = 1f)
  {
    animator.SetFloat("speed", newSpeed);

    //Debug.Log("animator speed " + newSpeed);
  }

  protected AnimationClip getClipByName(string nm)
  {
    AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;

    for (int i = 0; i < animationClips.Length; i++)
    {
      //if (nm == animationClips[i].name) return animationClips[i];
      if (animationClips[i].name.EndsWith(nm)) return animationClips[i];
    }

    //si l'animator a pas le clip ça veut dire qu'on veut jouer a partir d'où il en est
    //Debug.LogError("clip " + nm + " could not be found ?");
    return null;
  }

  /* si ça retourne -1 c'est que l'animator ne joue pas l'anim qu'on demande */
  protected int getLayerIndexByStateName(string nm)
  {
    for (int i = 0; i < animator.layerCount; i++)
    {
      //est déjà en train de jouer l'anim qu'on cherche
      if (animator.GetCurrentAnimatorStateInfo(i).IsName(nm)) return i;
    }
    return -1;
  }

  public string toString()
  {
    string ct = "\n[AnimatorPlayer]";
    
    ct += "\n time ratio : " + getTimeRatio() + " / 1";
    ct += "\n time (sec) : " + (animInfo.normalizedTime * animTimeLength) + " / " + animTimeLength;
    ct += "\n speed : " + animator.GetFloat("speed");
    
    ct += getTreeInfoToString();
    
    return ct;
  }



  protected string getTreeInfoToString()
  {
    string ct = "\n[module = " + owner.name + " , animator = " + animator.name + "]";
    
    ct += "\nanimTimePrevious : " + animTimePrevious;

    for (int i = 0; i < animator.layerCount; i++)
    {
      ct += "\n  layer #" + i + " of name : " + animator.GetLayerName(i) + " | normTime : " + animator.GetCurrentAnimatorStateInfo(i).normalizedTime;

      AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(i);
      if (clips.Length == 0) ct += "\n    no active clips";
      else
      {
        for (int j = 0; j < clips.Length; j++)
        {
          ct += "\n    clip | " + clips[j].clip.name;
        }
      }

    }

    return ct;
  }

}
