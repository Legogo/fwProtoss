using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using halper.visibility;

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
//canvas doit etre choppé après le build
//dans le cas de l'utilisation de ResourceManager un element d'UI va etre mit enfant du canvas de l'objet qui est dupliqué
//il est donc pas déjà enfant au build()
/// </summary>

namespace ui
{
  using halper.visibility;

  abstract public class UiAnimation : UiObject, scaffolder.iScaffDebug
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
    public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    
    override protected void created()
    {
      base.created();

      rec = GetComponent<RectTransform>();

      if (playOnSetup) play();
    }

    public UiAnimation play()
    {
      reset();

      animTimer = 0f;

      animStart();

      //Debug.Log(name + " play !");
      //Debug.Log("  L " + animTimer + " / " + animationLength);

      return this;
    }

    virtual public void reset()
    {
      animTimer = 0f;
    }

    private void Update()
    {
      
      if (animTimer < animationLength)
      {
        animTimer += Time.deltaTime;

        //Debug.Log(name + " , " + animTimer+" / "+animationLength, transform);

        if (animTimer >= animationLength)
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

    virtual protected void animStart()
    {
      gameObject.SetActive(true);
    }

    virtual protected void animEnd()
    {
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

    virtual public void clean()
    {
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

    public string stringify()
    {
      string ct = string.Empty;
      ct += "\ntimer : " + animTimer + " / " + animationLength;
      return ct;
    }
  }

}
