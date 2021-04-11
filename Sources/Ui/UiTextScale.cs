using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ui
{
  public class UiTextScale : UiAnimation
  {

    protected Text txt;

    int originalFontSize = 10;

    public int fontSizeTarget = 10;

    protected override void created()
    {
      base.created();

      txt = GetComponent<Text>();

      originalFontSize = txt.fontSize;
    }

    protected override void updateAnimationProcess()
    {

      txt.fontSize = Mathf.FloorToInt(Mathf.Lerp(originalFontSize, fontSizeTarget, getProgress()));
    }

    override public void reset()
    {
      base.reset();

      txt.fontSize = originalFontSize;
    }

  }

}
