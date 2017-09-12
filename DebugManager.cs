using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour {

  public bool show = false;
  
  GUIStyle skin;
  Rect guiRec = new Rect(15f,15f,0f,0f);
  
  void Update() {
    if (Input.GetKeyUp(KeyCode.RightShift))
    {
      show = !show;
    }
  }

#if UNITY_EDITOR

  void OnGUI() {
    
    if (!show) return;

    if(skin == null) {
      skin = new GUIStyle();
      skin.fontSize = 35;
      skin.normal.textColor = Color.red;
      skin.fontStyle = FontStyle.Bold;
    }

    guiRec.width = Camera.main.pixelWidth;
    guiRec.height = Camera.main.pixelWidth;

    string str = getContent();
    GUI.Label(guiRec, str, skin);
  }

#endif

  virtual protected string getContent()
  {
    return "";
  }

}
