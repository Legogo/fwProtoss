using UnityEngine;
using UnityEngine.UI;
using fwp.input;

public class VersionToggle : EngineObject
{
  protected int count = 0;
  protected HelperScreenTouchSequenceSolver sequence;

  protected HelperVisibleUi vui;

  protected override VisibilityMode getVisibilityType()
  {
    return VisibilityMode.UI;
  }

  protected override void build()
  {
    base.build();

    HelperVisibleUi h = (HelperVisibleUi)visibility;
    h.setTextLabel(VersionManager.getFormatedVersion());

  }

  protected override void setup()
  {
    base.setup();

    subscribeToTouchRelease(touch);

    sequence = new HelperScreenTouchSequenceSolver(new Rect[]
    {
      new Rect(0.9f, 0.1f, 0.1f, 0.1f),
      new Rect(0.9f, 0.1f, 0.1f, 0.1f)
    });

    vui = visibility as HelperVisibleUi;
    if(vui == null)
    {
      Debug.LogError(name + " need to be setup as VisibilityUI");
    }

    visibility.hide();

    //if (!Debug.isDebugBuild) visibility.hide();
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
      //Debug.Log(name + " count at " + count);
    }

    if(count > 1)
    {
      //Debug.Log(name+" toggle !");
      if (visibility.isVisible()) visibility.hide();
      else visibility.show();

      count = 0;
    }
  }
  
}
