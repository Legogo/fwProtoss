using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiProgressBarSplit : UiProgressBar {
  
  Image[] renders;
  public int slots = 1;

  protected override void build()
  {
    base.build();
    fetchRefs();
  }

  protected void fetchRefs()
  {

    if (renders != null && renders.Length > 0) return;

    //search for filled image
    Image[] imgs = GetComponentsInChildren<Image>();

    //only keep 'fill' type
    List<Image> tmp = new List<Image>();
    for (int i = 0; i < imgs.Length; i++)
    {
      if (imgs[i].type == Image.Type.Filled)
      {
        tmp.Add(imgs[i]);
      }
    }

    renders = tmp.ToArray();
  }

  private void OnValidate()
  {
    if (Application.isPlaying) return;

    fetchRefs();
    applyProgress();
  }

  public UiProgressBarSplit setSlotsCount(int newCount)
  {
    //cannot be less than 1
    if (newCount < 1) return this;

    //Debug.Log("new count " + newCount);

    //compute old progress to new one (based on new count)
    progressiveStep = (progressiveStep * slots) / newCount;
    progressiveTarget = (progressiveTarget * slots) / newCount;
    
    slots = newCount;
    applyProgress();

    return this;
  }

  public override void applyProgress()
  {
    if (renders.Length <= 0) return;

    float step = (1f / slots); // 4 = 0.25f
    float subProg = 0f;

    if (progressiveStep > 0f) {
      subProg = ((progressiveStep * 100f) % (step * 100f)) / 100f;
    }

    //Debug.Log("progress : " + progressiveStep + " , slots : " + slots);
    //Debug.Log("step : " + step + " , subProg : " + subProg);

    int idx = Mathf.FloorToInt(progressiveStep * slots);

    for (int i = 0; i < renders.Length; i++)
    {
      renders[i].fillAmount = (i < idx) ? 1f : 0f;
      renders[i].enabled = (i < slots);
    }

    if (idx > renders.Length - 1) renders[renders.Length - 1].fillAmount = 1f;
    else renders[idx].fillAmount = subProg / step;
  }

  /* transform current progress (based on current slots count) to progress with another slots counts */
  public float getRelativeProgress(int refSlotsCount)
  {
    float progress = (progressiveStep * slots) / refSlotsCount;
    return progress;
  }

}
