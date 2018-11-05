using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiProgressBar : EngineObject {

  protected HelperVisibleUi render;
  protected Image renderImg;
  protected RectTransform renderRectTr;

  [Header("progressive logic")]
  public float progressiveSpeed = 999f;

  [Range(0f,1f)]
  public float progressiveStep = 1f; // where it's at

  protected float progressiveTarget = 1f; // where it wanna go
  
  protected override void build()
  {
    base.build();

    progressiveStep = 1f;
    progressiveTarget = 1f;
    
  }

  protected override void setup()
  {
    base.setup();

    render = visibility as HelperVisibleUi;
    renderImg = render.getImage();
    renderRectTr = renderImg.GetComponent<RectTransform>();

    if(renderImg.type != Image.Type.Filled)
    {
      Debug.LogError("asking to modify some ui element as progress bar but target ui element is not setup as fill ?");
    }
  }
  
  protected override VisibilityMode getVisibilityType()
  {
    return VisibilityMode.UI;
  }
  
  private void OnValidate()
  {
    if (Application.isPlaying) return;
    
    renderImg = transform.GetComponent<Image>();
    renderRectTr = renderImg.GetComponent<RectTransform>();

    applyProgress();
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
  
  public bool isFull() { return renderImg.fillAmount >= 1f; }
  public bool isEmpty() { return renderImg.fillAmount <= 0f; }

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
    updateStep();
  }

  virtual public void applyProgress()
  {
    renderImg.fillAmount = progressiveStep;
  }

  public float getProgress()
  {
    return progressiveStep;
  }
  
  public void follow(Vector2 targetPosition)
  {
    renderRectTr.position = Camera.main.WorldToScreenPoint(targetPosition);
    //Debug.Log(pivot.position);
  }
}
