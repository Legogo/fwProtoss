using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UiAnimation : EngineObject
{
  protected RectTransform _owner;
  public float animationLength = 1f;
  protected float animTimer = 0f;

  public AnimationCurve curve;
  public Action onAnimationDone;

  protected override void build()
  {
    base.build();
    _owner = GetComponent<RectTransform>();
    setFreeze(true);
  }

  public void play()
  {
    reset();
    setFreeze(false);

    animTimer = 0f;

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
        if (onAnimationDone != null) onAnimationDone();
      }
    }
    

    updateUiAnimation();
  }
  
  protected float getProgress()
  {
    //return Mathf.Lerp(0f, animationLength, animTimer);
    return curve.Evaluate(animTimer / animationLength);
  }

  virtual protected void updateUiAnimation()
  {
    //Debug.Log(name + " "+GetType()+" update ...");
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
