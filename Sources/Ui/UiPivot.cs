using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// meant to give tools to setup ui on screen
/// </summary>

public class UiPivot
{
  protected RectTransform rec;
  
  public UiPivot(Transform tr)
  {
    rec = tr.GetComponent<RectTransform>();
  }

  public void setOnScreen(int x, int y)
  {
    Vector3 pos = rec.position;
    pos.x = x;
    pos.y = y;

    rec.position = pos;
  }

}
