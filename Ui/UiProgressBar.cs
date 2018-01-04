using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiProgressBar : EngineObject {

  protected RectTransform pivot;
  protected Image render;

  [Header("progressive logic")]
  public float progressiveSpeed = 0f;
  protected float progressiveTarget = 0f;
  
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

    progressiveTarget = 0f;
  }

  public override void updateEngine()
  {
    base.updateEngine();

    if (progressiveSpeed > 0f)
    {
      render.fillAmount = Mathf.MoveTowards(render.fillAmount, progressiveTarget, Time.deltaTime * progressiveSpeed);
    }
  }

  public void follow(Vector2 targetPosition)
  {
    pivot.position = Camera.main.WorldToScreenPoint(targetPosition);
    //Debug.Log(pivot.position);
  }

  public bool isFull() { return render.fillAmount >= 1f; }
  public bool isEmpty() { return render.fillAmount <= 0f; }

  public void setProgress(float newProgress)
  {
    if (progressiveSpeed > 0f)
    {
      progressiveTarget = newProgress;
    }
    else
    {
      render.fillAmount = newProgress;
    }
  }
}
