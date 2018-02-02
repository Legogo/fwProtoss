using System;
using UnityEngine;
using System.Collections.Generic;

public class AnimatorPlayer
{
  public const string noneState = "none";
  protected Transform owner;
  protected Animator animator;
  protected AnimatorStateInfo animInfo;

  //dans le cas ou l'animation est controlée par un EngineRange faut pas que la timeline continue quand on arrive a 1
  //utilisé par PropAnimatorInteractor
  public bool neverEnd = false;

  protected AnimationClip state_clip;
  protected bool anim_has = false;
  protected bool anim_at_end = false;
  protected int layer_active_index = 0;
  
  private float animTimeLength = 0f;
  private float animCurrentTime = 0f;
  private float animTimePrevious = 0f;

  public Action onAnimEnd;
  
  protected AnimatorPlayer[] others; // la liste des autres AnimatorPlayer qui pointent sur le même Animator

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

  public void reset()
  {
    anim_has = false;
    anim_at_end = false;
    animTimePrevious = -1f; // pour que ça réagisse même quand la valeur de start == end
  }

  public void setup() {
    
    if (animator == null)
    {
      Debug.LogError("~AnimatorPlayer~ no animator for " + owner.name+" (was destroyed ?)", owner.gameObject);
      return;
    }
    
    //remettre la speed a 0 au cas ou
    stop();
  }
  
  public void launch(string stateName, float startTimeRatio = 0f)
  {
    
    //dans le cas de la scène de la main il faut pouvoir desactiver l'animator pour éviter de bloquer les transforms
    //il faut GARDER cette fonction
    if (!animator.enabled) animator.enabled = true;
    
    reset();
    
    state_clip = getClipByName(stateName);
    
    if (state_clip != null)
    {
      animTimeLength = 1f * state_clip.length;
      animCurrentTime = (animTimeLength * startTimeRatio) * 1f;

      //setup start value into anim
      //Debug.Log(getStateName());

      animator.Play(stateName, layer_active_index, startTimeRatio);
      //animator.Play(getStateName(), 0, startTimeRatio);
      
    }
  }
  
  /* must be called by parent to make the animation fo forward */
  public void update_animation_setFrame(float aimTime, float speed)
  {
    animInfo = animator.GetCurrentAnimatorStateInfo(layer_active_index);
    animTimeLength = animInfo.length;

    float target = (animTimeLength * aimTime);
    float stepSpeed = GameTime.deltaTime * speed;
    
    //Debug.Log("  current "+animCurrentTime+" / "+target+" ("+stepSpeed+")");
    animCurrentTime = Mathf.MoveTowards(animCurrentTime, target, stepSpeed);
    
    float timeNorm = animCurrentTime / animTimeLength;
    //Debug.Log("  result " + animCurrentTime + " / " + animTimeLength + " = " +timeNorm);

    setAtTime(timeNorm);
  }

  protected void forceAtTimeByRatio(float timeRatio) {
    animCurrentTime = timeRatio * animTimeLength;
    setAtTime(timeRatio);
  }

  public void setAtTime(float timeRatio) {

    //Debug.Log(timeRatio);

    animator.Play(animInfo.fullPathHash, -1, timeRatio);
    //Debug.Log(animInfo.fullPathHash + " -> " + endTimeRatio);
  }

  public bool isClipLooping() {
    return state_clip.isLooping;
  }

  public bool checkAnimAtEnd(float endTime)
  {
    if (neverEnd) return false;
    //return Mathf.Approximately(Mathf.Clamp01(animInfo.normalizedTime), endTime);
    return Mathf.Approximately(Mathf.Clamp01(getTimeRatio()), endTime);
  }

  public bool isDone()
  {
    return anim_at_end;
  }

  /* stop le player mais ne touche pas la vitesse du controller */
  public void killPlayer() {
    
    if (anim_at_end)
    {
      Debug.LogWarning(owner.name+" already setup as at end");
      return;
    }

    anim_at_end = true;
  }

  public void stop()
  {
    //Debug.Log("stop " + owner.GetInstanceID());
    anim_at_end = true; // force finish
    setAnimatorSpeed(0f);
  }

  public void playNone()
  {
    animator.Play("none", layer_active_index);
  }
  
  /* la layer qui a un normTime en cours ]0,1[ */
  protected int getActiveLayerIndex() {
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

  protected void setAnimatorSpeed(float newSpeed = 1f)
  {
    if (animator == null) Debug.LogError("no animator ?", owner.gameObject);
    animator.SetFloat("speed", newSpeed);
    //Debug.Log(animator.name+" , animator speed to " + newSpeed, animator.gameObject);
  }

  protected AnimationClip getClipByName(string nm)
  {
    AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;

    for (int i = 0; i < animationClips.Length; i++)
    {
      if (nm == animationClips[i].name) return animationClips[i];
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

    ct += "\n anim_at_end ? " + anim_at_end + " , anim_has ? " + anim_has + " , anim loop ? " + animInfo.loop;
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
