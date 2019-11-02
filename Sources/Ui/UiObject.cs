using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiObject : EngineObject
{
  protected HelperVisibleUi hVisible;
  UiPivot pivot;
  
  protected override void build()
  {
    base.build();
    pivot = new UiPivot(transform);

    hVisible = visibility as HelperVisibleUi;
  }
  
  protected override void setup()
  {
    base.setup();

    //show();
    hide();
  }

  public void setupScreenPosition(float x, float y)
  {
    Debug.Log(name + " , " + x + " x " + y);
    pivot.setOnScreen(x, y); // proportional
  }

  protected override VisibilityMode getVisibilityType()
  {
    return VisibilityMode.UI;
  }

  sealed public override void updateEngine()
  {
    base.updateEngine();

    //Debug.Log(Time.frameCount + " update " + name, transform);

    updateUi();
  }

  virtual protected void updateUi()
  { }

  public bool isVisible() { return visibility.isVisible(); }
  virtual public void show() { visibility.show(); }
  virtual public void hide() { visibility.hide(); }
}
