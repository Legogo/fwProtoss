using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleLabelCounter : ModuleLabel {
  
  public float count = 0;

  public float count_target_speed = 0f;
  protected float count_target = -1;

  public Vector2 countClampLimit = Vector2.zero;
  public Vector2 countRandomRange = Vector2.zero;

  protected float score_target_time = 0.01f;
  protected float score_target_timer = 0f;
  protected int score_target = 0;

  public override void restart()
  {
    base.restart();
    setCount(0);
  }
  
  protected override void update()
  {
    base.update();

    if (count_target_speed > 0f) updateCountTarget();
  }

  protected void updateCountTarget()
  {

    if (score_target_timer < score_target_time)
    {
      score_target_timer += Time.deltaTime;

      if (score_target_timer >= score_target_time)
      {
        score_target_timer = 0f;

        //animation du score
        if (count < count_target)
        {
          addToCount(1);
          updateTextWithCount();
        }
      }
    }

  }

  protected void updateTextWithCount()
  {
    updateLabel(getCount().ToString());
  }

  public void addToCount(float step)
  {
    setCount(getCount() + step);
  }

  protected bool isUsingProgressiveTargetCount() { return count_target_speed > 0f; }

  virtual public void setCount(float newCount)
  {
    
    //clamp value
    if(countClampLimit.sqrMagnitude != 0)
    {
      newCount = Mathf.Clamp(newCount, countClampLimit.x, countClampLimit.y);
    }
    
    //re-assign
    if (isUsingProgressiveTargetCount())
    {
      count_target = newCount;
    }else
    {
      count = newCount;
    }

    //Debug.Log(name+" | (progressive ? "+isUsingProgressiveTargetCount()+") "+count + " , " + count_target + " = " + getCount(), gameObject);
    updateTextWithCount();
  }

  public void assignRandomValueByLimits()
  {
    int val = Random.Range((int)countRandomRange.x, (int)countRandomRange.y+1);

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

  public float getCount() { return (isUsingProgressiveTargetCount()) ? count_target : count; }
  public int getIntCount() { return Mathf.FloorToInt(getCount()); }
}
