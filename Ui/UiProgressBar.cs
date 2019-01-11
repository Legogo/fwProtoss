using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiProgressBar : EngineObject {

  public Image renderImg;
  protected RectTransform rtPivot;

  [Header("progressive logic")]
  public float progressiveSpeed = 999f;

  [Range(0f,1f)]
  public float progressiveStep = 1f; // where it's at

  protected float progressiveTarget = 1f; // where it wanna go

  protected override VisibilityMode getVisibilityType()
  {
    return VisibilityMode.UI;
  }

  protected override void build()
  {
    base.build();

    progressiveStep = 1f;
    progressiveTarget = 1f;
  }

  protected override void setup()
  {
    base.setup();
    
    if(renderImg == null)
    {
      renderImg = GetComponent<Image>();
      if (renderImg == null) renderImg = GetComponentInChildren<Image>();
    }

    rtPivot = GetComponent<RectTransform>();

#if UNITY_EDITOR
    if (renderImg.type != Image.Type.Filled)
    {
      Debug.LogError("asking to modify some ui element as progress bar but target ui element is not setup as fill ?");
      UnityEditor.Selection.activeGameObject = renderImg.gameObject;
    }
#endif
    //visibility.hide();
  }
  
  public void setVisibility(bool flag)
  {
    if (gameObject.activeSelf != flag) gameObject.SetActive(flag);
  }

  public bool isVisible()
  {
    return gameObject.activeSelf;
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
    progressiveTarget = Mathf.Clamp01(progressiveTarget);

    //Debug.Log(name+" , new progress is " + newProgress, transform);

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
  
  public void follow(Vector2 targetWorldPosition)
  {
    rtPivot.position = Camera.main.WorldToScreenPoint(targetWorldPosition);
    //Debug.Log(pivot.position);
  }


#if UNITY_EDITOR
  private void OnValidate()
  {
    if (Application.isPlaying) return;

    if(renderImg == null) renderImg = transform.GetComponent<Image>();
    rtPivot = renderImg.GetComponent<RectTransform>();

    applyProgress();
  }
#endif

}
