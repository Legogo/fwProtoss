using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// it's interesting to separate slots[] from usable count to be able to have a variable progress bar size
/// </summary>

public class UiProgressBarSplit : UiProgressBar {
  
  public Image[] slots;
  public int slotCount = 1; // to keep, see documentation

  protected override void build()
  {
    base.build();
    fetchRefs();
  }

  protected void fetchRefs()
  {
    if (Application.isPlaying)
    {
      if (slots != null && slots.Length > 0) return;
    }

    //search for filled image
    Image[] imgs = HalperComponentsGenerics.getComponentsInChildren<Image>(transform);
    //Image[] imgs = transform.GetComponentsInChildren<Image>();

    //only keep 'fill' type
    List<Image> tmp = new List<Image>();
    for (int i = 0; i < imgs.Length; i++)
    {
      if (imgs[i].type == Image.Type.Filled)
      {
        tmp.Add(imgs[i]);
      }
    }

    slots = tmp.ToArray();
    //Debug.Log("fetched " + renders.Length + " renders");
    //for (int i = 0; i < renders.Length; i++) Debug.Log("  L " + renders[i].name, renders[i].transform);

  }

  public UiProgressBarSplit setSlotsCount(int newCount)
  {
    //cannot be less than 1
    if (newCount < 1) return this;

    //Debug.Log("new count " + newCount);

    //compute old progress to new one (based on new count)
    progressiveStep = (progressiveStep * slotCount) / newCount;
    progressiveTarget = (progressiveTarget * slotCount) / newCount;

    slotCount = newCount;
    applyProgress();

    return this;
  }

  public override void applyProgress()
  {
    if (slots.Length <= 0) return;

    float step = (1f / slotCount); // 4 = 0.25f
    float subProg = 0f;

    if (progressiveStep > 0f) {
      subProg = ((progressiveStep * 100f) % (step * 100f)) / 100f;
    }

    //Debug.Log("update progress on " + renders.Length+" renders / slots : "+slots);
    //Debug.Log("progress : " + progressiveStep);
    //Debug.Log("step : " + step + " , subProg : " + subProg);

    int idx = Mathf.FloorToInt(progressiveStep * slotCount);

    for (int i = 0; i < slots.Length; i++)
    {
      slots[i].fillAmount = (i < idx) ? 1f : 0f;
      slots[i].enabled = (i < slotCount);
    }

    //Debug.Log(progressiveStep+" , "+slotCount+" | "+ idx+" / "+slots.Length);

    if (idx > slots.Length - 1) slots[slots.Length - 1].fillAmount = 1f;
    else slots[idx].fillAmount = subProg / step;
  }

  /// <summary>
  /// add filling to progress bar by slot count
  /// </summary>
  /// <param name="qty"></param>
  public void addProgressSlotStep(float qty)
  {
    float step = 1f / slots.Length;
    addProgress(qty * step);
  }

  public void setProgressBySlotCount(int count)
  {
    float step = 1f / slots.Length;
    setProgress(step * count);
  }

  /* transform current progress (based on current slots count) to progress with another slots counts */
  public float getRelativeProgress(int refSlotsCount)
  {
    float progress = (progressiveStep * slotCount) / refSlotsCount;
    return progress;
  }

#if UNITY_EDITOR
  private void OnValidate()
  {
    if (Application.isPlaying) return;

    fetchRefs();
    applyProgress();

  }
#endif

}
