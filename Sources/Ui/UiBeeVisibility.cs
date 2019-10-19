using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiBeeVisibility
{
  protected Canvas canvas;

  public UiBeeVisibility(Transform tr)
  {
    canvas = tr.GetComponentInParent<Canvas>();
  }

  public bool isVisible() { return canvas.enabled; }
  public void show() { canvas.enabled = true; }
  public void hide() { canvas.enabled = false; }
}
