using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// https://docs.unity3d.com/ScriptReference/Animator.html
/// https://answers.unity.com/questions/425985/set-the-current-time-frame-of-a-mecanim-animation.html
/// </summary>

public class AnimatorWrapper
{
  Animator animator;

  string stateName = "";

  Action<string> onPlay;
  Action<string> onEnd;

  AnimatorStateInfo state; // default layer (0)
  AnimatorClipInfo[] infos;

  //float previousNormTime = 0f;
  //bool _isPlaying = false;

  IEnumerator pDelay;

  public AnimatorWrapper(Animator animator)
  {
    this.animator = animator;
    stop();
  }

  public AnimatorWrapper play(string playStateName, float delay = 0f, float speed = 1f)
  {
    if (isPlayingState(playStateName))
    {
      Debug.LogWarning(playStateName + " already playing");
      return this;
    }

    stateName = playStateName;
    
    animator.speed = speed;

    if (delay <= 0f) play(stateName);
    else {
      Debug.Log(animator.gameObject.name + " will play " + playStateName + " delayed " + delay);
      pDelay = processDelay(stateName, delay);
    }
    
    return this;
  }

  IEnumerator processDelay(string stateName, float delay)
  {

    while(delay > 0f)
    {
      delay -= Time.deltaTime;
      yield return null;
    }

    play(stateName);

    pDelay = null;
  }

  protected void play(string stateName)
  {
    
    if (!animator.enabled) animator.enabled = true;
    
    animator.Play(stateName, 0, 0f);

    //previousNormTime = 0f;
    update_infos();

    if (onPlay != null) onPlay(stateName);

    //Debug.Log("wrapper is playing " + stateName, animator.gameObject);
  }

  public AnimatorWrapper subscribe(Action<string> onPlay, Action<string> onEnd)
  {
    if (onPlay != null) this.onPlay += onPlay;
    if (onEnd != null) this.onEnd += onEnd;

    return this;
  }

  public AnimatorWrapper unsubscribe(Action<string> actToRemove)
  {
    if (onPlay != null) this.onPlay -= actToRemove;
    if (onEnd != null) this.onEnd -= actToRemove;

    return this;
  }

  public void update()
  {
    if (pDelay != null)
    {
      if (!pDelay.MoveNext()) pDelay = null;

      if (pDelay != null) return;
    }

    update_infos();

    if (!isPlayingState(stateName)) return;

    if (!state.loop)
    {

      //Debug.Log(getAnimatorInfo());

      if (state.normalizedTime >= 1f)
      {
        animator.Play(stateName, 0, 1f); // force at end frame

        //Debug.Log(animator.gameObject.name+" at end of animation " + stateName, animator.gameObject);

        if (onEnd != null) onEnd(stateName);
        stop();
      }
    }

  }

  protected void update_infos()
  {
    infos = animator.GetCurrentAnimatorClipInfo(0);
    state = animator.GetCurrentAnimatorStateInfo(0);
  }

  public void forceAtTime(float secTime, bool freeze = false)
  {
    animator.Play(stateName, 0, secTime * state.length);
    if (freeze) changeSpeed(0f);
  }

  public AnimatorWrapper changeSpeed(float newSpeed)
  {
    animator.speed = newSpeed;
    return this;
  }

  public bool isPlayingState(string stateName)
  {
    if (!animator.enabled) return false;
    if (this.stateName == stateName)
    {
      if (infos[0].clip.name == stateName)
      {
        return true;
      }
      return false;
    }
    return false;
  }

  public bool isPlaying()
  {
    if(stateName.Length <= 0) return false;
    return animator.speed > 0f;
  }

  public AnimatorWrapper stop()
  {
    animator.speed = 0f;
    
    stateName = "";

    //release transform
    if (animator.enabled) animator.enabled = false;

    //Debug.Log(animator.gameObject.name + " stop");
    //Debug.Log(getAnimatorInfo());

    return this;
  }

  public string getAnimatorInfo()
  {
    string ct = animator.name;

    ct += "\n enabled ? " + animator.enabled + " , speed : " + animator.speed;
    ct += "\n current statename : " + stateName;
    ct += "\n has delay ? " + (pDelay != null);

    bool isPlayingCurrent = isPlayingState(stateName);
    ct += "\n isPlayingState(" + stateName + ") " + isPlayingCurrent;
    if (!isPlayingCurrent)
    {
      ct += "\n    <Animator> enabled ? " + animator.enabled;
      ct += "\n    statename ? "+stateName;
      if(infos != null && infos.Length > 0) ct += "\n    clip playing ? " + infos[0].clip.name;
      ct += "\n    is done | loop : " + state.loop + " , norm time " + state.normalizedTime;
    }

    ct += "\n__state__";
    ct += "\nstate name : " + state.fullPathHash + " | loop : " + state.loop;
    ct += "\ntime : " + state.normalizedTime + " / length : " + state.length;

    //ct += "\nstate to string : " + state.ToString();

    ct += "\nplaying : " + isPlaying();

    if(infos != null)
    {
      for (int i = 0; i < infos.Length; i++)
      {
        ct += "\n ---";
        ct += "\nclip name : " + infos[i].clip.name;
        ct += "\nclip looping : " + infos[i].clip.isLooping;
        ct += "\nclip length : " + infos[i].clip.length;
      }
    }

    return ct;
  }
}