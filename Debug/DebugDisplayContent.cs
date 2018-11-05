using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDisplayContent : EngineObject {

  public bool startVisible = true;

  protected string ct = "";
  protected GUIStyle style;

  public Vector2 viewDimensions = new Vector2(Screen.width, Screen.height);
  public float viewScaleFactor = 1f;

  private Rect view;

  protected override void build()
  {
    base.build();

    view = new Rect(20, 20, Screen.width - 20, Screen.height - 20);

    if (style == null)
    {
      style = new GUIStyle();
    }
  }

  protected override void setup()
  {
    base.setup();
    enabled = startVisible;
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

  /// <summary>
  /// draw gui stuff
  /// </summary>
  virtual protected void process()
  {
  }

  private void OnGUI()
  {
    if (!enabled) return;

    viewDimensions.x = Screen.width;
    viewDimensions.y = Screen.height;
    viewScaleFactor = Mathf.Max(viewScaleFactor, 0.1f);
    Vector3 dim = new Vector3(Screen.width / (viewDimensions.x * viewScaleFactor), Screen.height / (viewDimensions.y * viewScaleFactor), 1);

    GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, dim);

    setupFont();
    process();

    GUI.Label(view, ct, style);
  }
  
}
