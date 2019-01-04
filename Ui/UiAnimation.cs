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


//canvas doit etre choppé après le build
//dans le cas de l'utilisation de ResourceManager un element d'UI va etre mit enfant du canvas de l'objet qui est dupliqué
//il est donc pas déjà enfant au build()

/// </summary>

abstract public class UiAnimation : EngineObject
{
  protected float animTimer = 0f;
  public Action onAnimationDone;

  //protected Canvas canvas; // it shouldn't manage it's own canvas
  protected RectTransform rec;
  public HelperVisibleUi hVisibility;

  public bool playOnSetup = false;
  public bool loop = false;
  public bool destroyOnDone = true;

  [Header("basic")]
  public float animationLength = 1f;
  public AnimationCurve curve = AnimationCurve.Linear(0f,0f,1f,1f);
  
  protected override void build()
  {
    base.build();
    rec = GetComponent<RectTransform>();

    //somehow MUST BE in build() and not setup()
    setFreeze(true);
  }
  
  protected override VisibilityMode getVisibilityType()
  {
    return VisibilityMode.UI;
  }

  protected override void setupEarly()
  {
    base.setupEarly();

    hVisibility = visibility as HelperVisibleUi;

    if (playOnSetup) play();
  }

  public UiAnimation play()
  {
    reset();
    setFreeze(false);

    animTimer = 0f;

    animStart();

    //Debug.Log(name + " play !");
    //Debug.Log("  L " + animTimer + " / " + animationLength);

    return this;
  }
  
  virtual public void reset()
  {
    animTimer = 0f;
    setFreeze(true);
  }

  sealed public override void updateEngine()
  {
    base.updateEngine();
    
    if (isFreezed()) return;

    if(animTimer < animationLength)
    {
      animTimer += Time.deltaTime;

      //Debug.Log(name + " , " + animTimer+" / "+animationLength, transform);

      if(animTimer >= animationLength)
      {
        animTimer = animationLength;

        updateAnimationProcess(); // one last time to match end state

        animEnd();
        return;
      }
    }

    updateAnimationProcess();
  }

  abstract protected void updateAnimationProcess();
  
  protected float getProgress()
  {
    //return Mathf.Lerp(0f, animationLength, animTimer);
    return curve.Evaluate(animTimer / animationLength);
  }

  virtual protected void animStart() {
    gameObject.SetActive(true);
  }

  virtual protected void animEnd() {
    //if (ownerCanvas != null) ownerCanvas.enabled = false;

    if (onAnimationDone != null) onAnimationDone();

    if (loop)
    {
      play();
    }

    if (destroyOnDone)
    {
      GameObject.Destroy(gameObject);
    }
  }

  virtual public void clean() {
    gameObject.SetActive(false);
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
