using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 2017-10-30
/// Base class manage progress and call updateUiAnimation during animation progress
/// getProgress() returns evaluated (curve) percentage of animation progress
/// owner var is RectTransform of carry
/// </summary>

abstract public class UiAnimation : EngineObject
{
  protected Canvas ownerCanvas;
  protected RectTransform owner;

  public float animationLength = 1f;
  protected float animTimer = 0f;

  public AnimationCurve curve;
  public Action onAnimationDone;

  protected override void build()
  {
    base.build();
    owner = GetComponent<RectTransform>();
    
    ownerCanvas = GetComponent<Canvas>();
    if (ownerCanvas == null) ownerCanvas = GetComponentInParent<Canvas>();

    setFreeze(true);
  }

  public void play()
  {
    reset();
    setFreeze(false);

    animTimer = 0f;

    animStart();
    //Debug.Log(name + " play !");
  }

  virtual public void reset()
  {
    animTimer = 0f;
    setFreeze(true);
  }

  public override void updateEngine()
  {
    base.updateEngine();

    if (isFreezed()) return;

    if(animTimer < animationLength)
    {
      animTimer += Time.deltaTime;
      if(animTimer >= animationLength)
      {
        animTimer = animationLength;
        animEnd();
      }
    }
    

    animUpdate();
  }
  
  protected float getProgress()
  {
    //return Mathf.Lerp(0f, animationLength, animTimer);
    return curve.Evaluate(animTimer / animationLength);
  }

  virtual protected void animStart() {
    if (ownerCanvas != null) ownerCanvas.enabled = true;
  }

  virtual protected void animUpdate()
  {
    //Debug.Log(name + " "+GetType()+" update ...");
  }

  virtual protected void animEnd() {
    //if (ownerCanvas != null) ownerCanvas.enabled = false;

    if (onAnimationDone != null) onAnimationDone();
  }

  virtual public void clean() {
    if (ownerCanvas != null) ownerCanvas.enabled = false;
  }

  static public void killAll()
  {
    UiAnimation[] anims = GameObject.FindObjectsOfType<UiAnimation>();
    for (int i = 0; i < anims.Length; i++)
    {
      anims[i].reset();
    }
  }
}
