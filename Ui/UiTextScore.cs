using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTextScore : ArenaObject {

  public string labelSuffix = "";
  
  protected float score = 0f;
  protected Text label;

  protected override void setup()
  {
    base.setup();
    label = (visibility as HelperVisibleUi).getText();
  }

  public override void arena_round_restart()
  {
    base.arena_round_restart();
    score = 0f;
    updateVisual();
  }

  public void addScore(float step)
  {
    score += step;
    updateVisual();
  }

  virtual protected void updateVisual()
  {
    label.text = score + labelSuffix;
  }
}
