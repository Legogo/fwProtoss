using UnityEngine;
using UnityEngine.UI;

public class VersionToggle : EngineObject
{
  protected int count = 0;

  protected override void setup()
  {
    base.setup();

    HelperVisibleUi h = (HelperVisibleUi)visibility;
    h.setTextLabel(VersionManager.getFormatedVersion());

    subscribeToInput(touch);

    visibility.hide();
  }

  [ContextMenu("apply current version")]
  protected void updateText()
  {
    GetComponent<Text>().text = VersionManager.getFormatedVersion();
  }

  protected void touch(InputTouchFinger finger)
  {
    Vector2 pos = finger.screenProportional;
    //Debug.Log(pos);

    if(pos.x > 0.9f && pos.y < 0.1f)
    {
      count++;
    }

    if(count > 1)
    {
      //Debug.Log("toggle !");
      if (visibility.isVisible()) visibility.hide();
      else visibility.show();

      count = 0;
    }
  }
  
}
