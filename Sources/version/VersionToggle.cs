using UnityEngine;
using UnityEngine.UI;
using halper.visibility;
using inputeer;

public class VersionToggle : MonoBehaviour
{
  protected int count = 0;
  protected HelperScreenTouchSequenceSolver sequence;

  public bool hideOnStartup = true;

  HelperVisibleUi h;

  private void Awake()
  {
    h = (HelperVisibleUi)HelperVisible.createVisibility(this, VisibilityMode.UI);
    h.setTextLabel(VersionManager.getFormatedVersion());

    h.show();
  }

  private void Start()
  {
    //subscribeToTouchRelease(touch);

    sequence = new HelperScreenTouchSequenceSolver(new Rect[]
    {
      new Rect(0.9f, 0.1f, 0.1f, 0.1f),
      new Rect(0.9f, 0.1f, 0.1f, 0.1f)
    });

#if !debug
    Debug.Log("debug is off : killing version number on startup");
    hideOnStartup = true;
#endif

    if (hideOnStartup)
    {
      h.hide();
    }

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
      if (h.isVisible()) h.hide();
      else h.show();

      count = 0;
    }
  }
  
}
