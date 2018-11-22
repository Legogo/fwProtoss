using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

public class ArenaMonitoring : EditorWindow
{

  [MenuItem("Tools/monitoring/arena")]
  static void init()
  {
    EditorWindow.GetWindow(typeof(ArenaMonitoring));
  }
  
  void OnGUI()
  {
    GUILayout.Label("~Protoss framework~ arena");

    if(!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || !Application.isPlaying)
    {
      GUILayout.Label("only at runtime");
      return;
    }

    ArenaManager am = ArenaManager.get();

    GUILayout.Label("status");
    if (am == null) return;
    
    GUILayout.Label("  menu ? " + am.isArenaStateMenu());
    GUILayout.Label("  live ? " + am.isArenaStateLive());
    GUILayout.Label("  end ? " + am.isArenaStateEnd());
    
    GUILayout.Label("arena manager has "+am.arenaObjects.Count+" objects");
    for (int i = 0; i < am.arenaObjects.Count; i++)
    {
      GUILayout.Label("  #" + i + " "+am.arenaObjects[i]);
    }

  }
  
}
