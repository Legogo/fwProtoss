using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 2017-10-30
/// 
/// Base class manage progress and call updateUiAnimation during animation progress
/// getProgress() returns evaluated (curve) percentage of animation progress
/// owner var is RectTransform of carry
/// 
/// must setup : animationLength
/// 
/// play, reset
/// animStart
/// animUpdate
/// animEnd
/// clean
/// 
/// </summary>

abstract public class UiAnimation : EngineObject
{
  protected Canvas canvas;
  protected RectTransform rec;

  protected float animTimer = 0f;
  public Action onAnimationDone;

  [Header("basic")]
  public float animationLength = 1f;
  public AnimationCurve curve = AnimationCurve.Linear(0f,0f,1f,1f);
  
  protected override void build()
  {
    base.build();
    rec = GetComponent<RectTransform>();
    
    setFreeze(true);
  }

  protected override void setupEarly()
  {
    base.setupEarly();

    //canvas doit etre choppé après le build
    //dans le cas de l'utilisation de ResourceManager un element d'UI va etre mit enfant du canvas de l'objet qui est dupliqué
    //il est donc pas déjà enfant au build()
    canvas = GetComponent<Canvas>();
    if (canvas == null) canvas = GetComponentInParent<Canvas>();

    if (canvas == null)
    {
      Debug.LogError("no canvas for UiAnimation object " + name, transform);
    }

  }

  public UiAnimation play()
  {
    reset();
    setFreeze(false);

    animTimer = 0f;

    animStart();
    //Debug.Log(name + " play !");

    return this;
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
        animUpdate();
        animEnd();
        return;
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
    if (canvas != null) canvas.enabled = true;
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
    if (canvas != null) canvas.enabled = false;
  }

  static public void killAll()
  {
    UiAnimation[] anims = GameObject.FindObjectsOfType<UiAnimation>();
    for (int i = 0; i < anims.Length; i++)
    {
      anims[i].reset();
    }
  }

  public override string toString()
  {
    string ct = base.toString();
    ct += "\ntimer : " + animTimer + " / " + animationLength;
    return ct;
  }
}
