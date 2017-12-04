using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Interfaces;
/*
  Seulement dans l'editeur
  Permet d'avoir de l'info sur un objet qu'on clique (qui hérite de l'interface IDebugSelection)
*/

namespace Interfaces
{
  public interface IDebugSelection
  {
    string toStringDebug();
  }
}

public class DebugSelection : MonoBehaviour
{

  public GameObject target;
  
  #if UNITY_EDITOR
  Rect guiRec = new Rect(0, 0, Screen.width, Screen.height);
  void OnGUI()
  {
    GameObject obj = target;
    
    if (obj == null) {
      if (Selection.activeGameObject == null) return;
      obj = Selection.activeGameObject;
    }

    if (obj == null) return;

    //guiHandleLabel(obj);
    guiLabel(obj);
  }

  void OnDrawGizmos() {
    GameObject obj = target;

    if (obj == null)
    {
      if (Selection.activeGameObject == null) return;
      obj = Selection.activeGameObject;
    }

    if (obj == null) return;

    guiHandleLabel(obj);
  }

  /* dessin dans la scene view en debug */
  GUIStyle style;
  void guiHandleLabel(GameObject obj) {
    IDebugSelection info = obj.GetComponent<IDebugSelection>();
    //Debug.Log(info.toStringDebug());
    Handles.color = Color.white;

    if (style == null)
    {
      style = new GUIStyle();
      style.fontSize = 20;
      style.normal.textColor = Color.white;
    }

    if (info != null) {
      Handles.Label(obj.transform.position + Vector3.right, info.toStringDebug(), style);
    }
  }

  void guiLabel(GameObject obj) {

    GUIStyle style = new GUIStyle();
    //style.fontSize = Mathf.FloorToInt((Screen.width * 1f / Screen.height * 1f) * 50f);
    style.fontSize = 20;

    float lineHeight = style.fontSize * 1.5f;
    style.normal.textColor = Color.red;

    guiRec.y = 0;
    GUI.Label(guiRec, "[DebugSelection]\n", style);

    string info = "";

    guiRec.x = 0;
    guiRec.y = lineHeight;

    IDebugSelection[] infos = obj.GetComponents<IDebugSelection>();
    for (int i = 0; i < infos.Length; i++)
    {
      info = "{"+infos[i].GetType()+"}\n"+infos[i].toStringDebug();
      
      //if (i > 0) guiRec.y += 30;
      GUI.Label(guiRec, info, style);

      if(i % 2 == 0)
      {
        guiRec.x = 500;
      }
      else
      {
        guiRec.y += 300;
        guiRec.x = 0;
      }
      
      //Debug.Log(info);
      //Debug.Log(info.Split('\n').Length + " = " + guiRec.y);

    }

  }
  #endif
}
