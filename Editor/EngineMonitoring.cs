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

    if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || !Application.isPlaying)
    {
      GUILayout.Label("only at runtime");
      return;
    }

    EngineManager em = EngineManager.get();

    if (em == null) return;

    SortedDictionary<int, List<EngineObject>> layers = EngineManager.eosLayers;

    for (int i = 0; i < layers.Count; i++)
    {
      GUILayout.Label("  #" + i + " count : " + layers[i].Count);
    }

    GUILayout.Label("mEngine loading ? " + EngineManager.isLoading());
    GUILayout.Label("mEngine live ? "+EngineManager.isLive());

    if(GUILayout.Button("select main camera"))
    {
      UnityEditor.Selection.activeGameObject = Camera.main.gameObject;
    }

    if (GUILayout.Button("select arena"))
    {
      ArenaManager am = ArenaManager.get();
      if(am != null) UnityEditor.Selection.activeGameObject = am.gameObject;
    }
  }
  
}
