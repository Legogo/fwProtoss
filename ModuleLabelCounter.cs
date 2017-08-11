using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleLabelCounter : ModuleLabel {
  
  public float count = 0;
  
  public Vector2 countClampLimit = Vector2.zero;
  public Vector2 countRandomRange = Vector2.zero;

  public override void restart()
  {
    base.restart();
    count = 0;
  }
  
  public void addToCount(int step)
  {
    setCount(count + step);
  }

  virtual public void setCount(float newCount)
  {
    count = newCount;
    if(countClampLimit.sqrMagnitude != 0)
    {
      count = Mathf.Clamp(count, countClampLimit.x, countClampLimit.y);
    }
    

    updateLabel("" + count);
  }

  public void assignRandomValueByLimits()
  {
    setCount(Random.Range((int)countRandomRange.x, (int)countRandomRange.y));
  }

  public bool isAtMax()
  {
    if (countClampLimit.sqrMagnitude == 0f) return false;
    return count >= countClampLimit.y;
  }
  public bool isEmpty()
  {
    return count < 1;
  }

  public float getCount() { return count; }
  public int getIntCount() { return Mathf.FloorToInt(count); }
}
