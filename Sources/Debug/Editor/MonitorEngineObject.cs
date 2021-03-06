﻿using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

public class EngineObjectMonitoring : EditorWindow
{

  [MenuItem("Monitoring/object inspector")]
  static void init()
  {
    EditorWindow.GetWindow(typeof(EngineObjectMonitoring));
  }

  string[] options;
  int dropdownIndex = 0;

  GameObject last;
  GUIStyle style;
  
  void OnGUI()
  {
    GUILayout.Label("~Protoss framework~ objects");

    if (!Application.isPlaying)
    {
      return;
    }

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

    if(obj != last)
    {
      dropdownIndex = 0;
      last = obj;
    }

    iDebugStringify[] list = getCandidates();
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

    if(dropdownIndex >= 0 && dropdownIndex < list.Length)
    {
      GUILayout.Label(list[dropdownIndex].stringify(), style);
    }
  }

  iDebugStringify[] getCandidates()
  {
    MonoBehaviour[] tmp = GameObject.FindObjectsOfType<MonoBehaviour>();
    List<iDebugStringify> output = new List<iDebugStringify>();
    for (int i = 0; i < tmp.Length; i++)
    {
      iDebugStringify inst = tmp[i] as iDebugStringify;
      if (inst != null) output.Add(inst);
    }
    return output.ToArray();
  }

  void Update()
  {
    if (EditorApplication.isPlaying && !EditorApplication.isPaused)
    {
      Repaint();
    }
  }

}

public interface iDebugStringify
{
  string stringify();
}
