using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDisplayContent : MonoBehaviour {

  protected string ct = "";
  protected GUIStyle style;
  
  private Rect view;

  virtual protected void Awake()
  {
    view = new Rect(20, 20, Screen.width - 20, Screen.height - 20);

    if (style == null)
    {
      style = new GUIStyle();
    }
  }

  virtual protected void setupFont()
  {
    
    style.fontSize = 30;

  }

  virtual protected void process()
  {

  }

  private void OnGUI()
  {
    if (!enabled) return;

    process();

    setupFont();

    GUI.Label(view, ct, style);
  }
  
}
