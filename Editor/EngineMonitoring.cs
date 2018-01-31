using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

public class EngineMonitoring : EditorWindow
{

  [MenuItem("Tools/monitoring/engine")]
  static void init()
  {
    EditorWindow.GetWindow(typeof(EngineMonitoring));
  }
  
  void OnGUI()
  {
    GUILayout.Label("~Protoss framework~");

    if (!Application.isPlaying)
    {
      GUILayout.Label("only at runtime");
      return;
    }

    GUILayout.Label("mEngine loading ? " + EngineManager.isLoading());
    GUILayout.Label("mEngine live ? "+EngineManager.isLive());
    
  }
  
}
