using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiProgressBar : EngineObject {

  protected RectTransform pivot;
  protected Image render;

  [Header("progressive logic")]
  public float progressiveSpeed = 999f;

  [Range(0f,1f)]
  public float progressiveStep = 1f; // where it's at

  protected float progressiveTarget = 1f; // where it wanna go
  
  protected override void build()
  {
    base.build();

    pivot = transform.GetChild(0).GetComponent<RectTransform>();

    //search for filled image
    Image[] imgs = GetComponentsInChildren<Image>();
    for (int i = 0; i < imgs.Length; i++)
    {
      if(imgs[i].type == Image.Type.Filled)
      {
        render = imgs[i];
      }
    }

    if (render == null) Debug.LogError("no filled image found for progress bar", gameObject);

    progressiveStep = 1f;
    progressiveTarget = 1f;
  }

  public override void onEngineSceneLoaded()
  {
    base.onEngineSceneLoaded();

    //Debug.Log("load "+progressiveStep + " / " + progressiveTarget);
  }

  public override void updateEngine()
  {
    base.updateEngine();

    updateStep();
  }

  protected void updateStep()
  {
    //Debug.Log(progressiveStep + " / " + progressiveTarget);

    //progress toward goal
    if (progressiveTarget != progressiveStep)
    {
      progressiveStep = Mathf.MoveTowards(progressiveStep, progressiveTarget, Time.deltaTime * progressiveSpeed);
      applyProgress();
    }
  }

  public void follow(Vector2 targetPosition)
  {
    pivot.position = Camera.main.WorldToScreenPoint(targetPosition);
    //Debug.Log(pivot.position);
  }

  public bool isFull() { return render.fillAmount >= 1f; }
  public bool isEmpty() { return render.fillAmount <= 0f; }

  public void addProgress(float step)
  {
    progressiveTarget += step;
    progressiveTarget = Mathf.Clamp01(progressiveTarget);

    //Debug.Log("new target " + progressiveTarget);
  }

  public void setProgress(float newProgress)
  {
    progressiveTarget = newProgress; // new goal
    applyProgress();
  }

  virtual public void applyProgress()
  {
    render.fillAmount = progressiveStep;
  }

  public float getProgress()
  {
    return progressiveStep;
  }
}
