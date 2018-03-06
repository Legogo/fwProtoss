using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTextScale : UiAnimation {

  protected Text txt;

  int originalFontSize = 10;

  public int fontSizeTarget = 10;
  
  protected override void build()
  {
    base.build();

    txt = GetComponent<Text>();

    originalFontSize = txt.fontSize;
  }
  
  protected override void animUpdate()
  {
    base.animUpdate();

    txt.fontSize = Mathf.FloorToInt(Mathf.Lerp(originalFontSize, fontSizeTarget, getProgress()));
  }
  
  override public void reset()
  {
    base.reset();

    txt.fontSize = originalFontSize;
  }

}
