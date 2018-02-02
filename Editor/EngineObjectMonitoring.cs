using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

public class EngineObjectMonitoring : EditorWindow
{

  [MenuItem("Tools/monitoring/objects")]
  static void init()
  {
    EditorWindow.GetWindow(typeof(EngineObjectMonitoring));
  }

  string[] options;
  int dropdownIndex = 0;

  GUIStyle style;
  
  void OnGUI()
  {
    GUILayout.Label("~Protoss framework~ objects");
    
    if(style == null)
    {
      style = new GUIStyle();
      style.richText = true;
    }
    /*
    if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || !Application.isPlaying)
    {
      GUILayout.Label("only at runtime");
      return;
    }
    */

    GameObject obj = Selection.activeGameObject;
    if (obj == null) return;

    EngineObject[] list = obj.GetComponents<EngineObject>();
    if (list.Length <= 0) return;

    List<string> newOptions = new List<string>();
    for (int i = 0; i < list.Length; i++)
    {
      newOptions.Add(list[i].GetType().ToString());
    }
    options = newOptions.ToArray();

    GUILayout.BeginHorizontal();
    dropdownIndex = EditorGUILayout.Popup("Objects", dropdownIndex, options, EditorStyles.popup);
    GUILayout.EndHorizontal();

    GUILayout.Label(list[dropdownIndex].toString(), style);
  }
  
}
