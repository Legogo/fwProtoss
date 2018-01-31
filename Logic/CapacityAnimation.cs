using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// blueprint for animation capacity integration
/// </summary>

abstract public class CapacityAnimation : LogicCapacity {

  protected Animator _anim;
  protected HelperVisibleSprite visibility_spr;
  
  protected bool _animCaptured = false;

  protected override void build()
  {
    base.build();

    visibility_spr = _owner.visibility as HelperVisibleSprite;

    _anim = gameObject.GetComponentsInChildren<Animator>().FirstOrDefault();
  }

  public override void updateLogic()
  {
    updateAnimation();
  }

  abstract protected void updateAnimation();

  public void PlayAnimOfName(string animName)
  {
    //Debug.Log(name+" playing " + animName);

    if (_anim == null) return;

    _anim.Play(animName);
  }

  public bool isPlaying(string animName)
  {
    if (_anim == null) return false;
    AnimatorStateInfo state = _anim.GetCurrentAnimatorStateInfo(0);
    if (state.IsName(animName)) return true;
    return false;
  }

  /* anim won't play something else */
  public void lockAnimation()
  {
    _animCaptured = true;
  }

  public void releaseAnimation()
  {
    _animCaptured = false;
  }

}
