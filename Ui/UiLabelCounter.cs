using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiLabelCounter : ArenaObject {

  public float count = 0f;
  protected float count_target = 0f;

  //permet de clamp la valeur quand elle est modifiée
  public Vector2 countClampLimit = Vector2.zero;

  //permet de demander une valeur aléatoire
  public Vector2 countRandomRange = Vector2.zero;

  public bool progressiveScoring = false;
  protected float score_target_time = 0.01f;
  protected float score_target_timer = 0f;
  protected int score_target = 0;

  [Header("layout")]
  public bool scoreBolded = false;
  public string prefix = "";

  public override void restart()
  {
    base.restart();
    score_target_timer = 0f;

    count_target = count = 0;
    updateTextWithCount();
  }

  public override void updateEngine()
  {
    base.updateEngine();

    if (progressiveScoring) updateCountTarget();
  }

  protected int solveScoreGap()
  {
    return Mathf.FloorToInt(count_target - count);
  }

  protected float solveProgressiveTime()
  {
    int limit = 50;
    int gap = Mathf.Min(limit, solveScoreGap());

    //plus le gap est grand, plus ça va vite
    float result = Mathf.Lerp(0.05f, 0.0f, Mathf.InverseLerp(0, limit, gap));

    //Debug.Log(count + " / " + count_target + " = " + gap + " = " + result);

    return result;
  }

  protected void updateCountTarget()
  {
    //Debug.Log(name + " update count target | "+score_target_timer+" < "+score_target_time+" | "+count+" / "+count_target);

    if (count >= count_target) return;

    if (count < count_target)
    {
      float step = Mathf.Lerp(1, 10, Mathf.InverseLerp(0, 20, solveScoreGap()));

      count += Mathf.FloorToInt(step);

      updateTextWithCount(true);
    }
  }

  protected void updateTextWithCount(bool forceCount = false)
  {
    string ct = "";

    //if(prefix.Length > 0) ct = prefix +" ";
    if (prefix.Length > 0) ct = prefix;

    string score = (forceCount) ? count.ToString() : getCount().ToString();

    if (scoreBolded) ct += "<b>" + score + "</b>";
    else ct += score;

    visibility.updateLabel(ct);
  }

  public void addToCount(float step)
  {
    setCount(getCount() + step);
  }

  public void setMin()
  {
    setCount(countRandomRange.x);
  }
  virtual public void setCount(float newCount)
  {

    //clamp value
    if (countClampLimit.sqrMagnitude != 0)
    {
      newCount = Mathf.Clamp(newCount, countClampLimit.x, countClampLimit.y);
    }

    //re-assign
    if (progressiveScoring)
    {
      count_target = newCount;
    }
    else
    {
      count = newCount;
    }

    //Debug.Log(name+" | (progressive ? "+isUsingProgressiveTargetCount()+") "+count + " , " + count_target + " = " + getCount(), gameObject);

    updateTextWithCount();
  }

  public void assignRandomValueByLimits()
  {
    int val = Random.Range((int)countRandomRange.x, (int)countRandomRange.y + 1);

    //Debug.Log(name+" | "+countRandomRange + " -> " + val, gameObject);

    setCount(val);
  }

  public bool isAtMax(float overrideMax = -1f)
  {
    float max = countClampLimit.y;

    if (overrideMax != -1f) max = overrideMax;
    else
    {
      if (countClampLimit.sqrMagnitude == 0f) return false;
    }

    return getCount() >= max;
  }
  public bool isEmpty()
  {
    return getCount() <= countClampLimit.x;
  }

  public float getCount() { return (progressiveScoring) ? count_target : count; }
  public int getIntCount() { return Mathf.FloorToInt(getCount()); }
}
