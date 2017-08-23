using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiAnimation : EngineObject
{
  protected RectTransform _owner;

  protected override void build()
  {
    base.build();
    _owner = GetComponent<RectTransform>();
    setFreeze(true);
  }

  public void play()
  {
    reset();
    setFreeze(false);
  }

  virtual public void reset()
  {
    setFreeze(true);
  }

  protected override void update()
  {
    base.update();

    if (isFreezed()) return;

    updateUiAnimation();
  }

  virtual protected void updateUiAnimation()
  {

  }
}
