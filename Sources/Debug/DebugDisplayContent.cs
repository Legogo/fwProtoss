using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDisplayContent : EngineObject {

  public bool startVisible = true;
  protected bool visible = false;

  [Header("align")]
  public Vector2 offset;
  public bool alignBottom;

  protected string ct = "";
  protected GUIStyle style;

  public Vector2 viewDimensions = new Vector2(Screen.width, Screen.height);
  public float viewScaleFactor = 1f;

  private Rect view;

  protected override void build()
  {
    base.build();
    
    view = new Rect(offset.x, offset.y, Screen.width, Screen.height);

    if (alignBottom)
    {
      view.y = Screen.height - offset.y;
    }

    if (style == null)
    {
      style = new GUIStyle();
    }

#if !debug
    startVisible = false;
#endif

    visible = startVisible;
  }
  
  protected void setupViewSize(int x, int y, int width, int height)
  {
    view.x = x;
    view.y = y;
    view.width = width;
    view.height = height;
  }

  virtual protected void setupFont()
  {
    
    style.fontSize = 30;

  }
  
  public void toggle()
  {
#if debug
    visible = !visible;
    enabled = visible;
#endif
  }

  /// <summary>
  /// draw gui stuff
  /// </summary>
  virtual protected void processGui()
  {
    ct = "";
  }

  private void OnGUI()
  {
    //Debug.Log(transform.name+" , "+visibility);

    if (!visible) return;

    viewDimensions.x = Screen.width;
    viewDimensions.y = Screen.height;
    viewScaleFactor = Mathf.Max(viewScaleFactor, 0.1f);
    Vector3 dim = new Vector3(Screen.width / (viewDimensions.x * viewScaleFactor), Screen.height / (viewDimensions.y * viewScaleFactor), 1);

    GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, dim);

    setupFont();
    processGui();

    GUI.Label(view, ct, style);
  }
  
}
