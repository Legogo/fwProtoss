using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

public class MonitorEngineObject : EditorWindow
{

  [MenuItem("Tools/monitoring/engine")]
  static void init()
  {
    EditorWindow.GetWindow(typeof(MonitorEngineObject));
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

    GUILayout.Label("layer neg ? " + (EngineManager.eosNegLayers != null));
    GUILayout.Label("layer pos ? " + (EngineManager.eosPosLayers != null));

    //SortedDictionary<int, List<EngineObject>> layers = EngineManager.eosLayers;
    /*
    for (int i = 0; i < layers.Count; i++)
    {
      GUILayout.Label("  #" + i + " count : " + layers[i].Count);
    }
    */

    GUILayout.Label("mEngine loading ? " + EngineManager.isLoading());
    GUILayout.Label("mEngine live ? "+EngineManager.isLive());

    if(GUILayout.Button("select main camera"))
    {
      UnityEditor.Selection.activeGameObject = Camera.main.gameObject;
    }
    
  }
  
}
