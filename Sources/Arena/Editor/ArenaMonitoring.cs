using UnityEngine;
using UnityEditor;

namespace fwp.arena
{
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

      if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || !Application.isPlaying)
      {
        GUILayout.Label("only at runtime");
        return;
      }

      GUILayout.Label("  engine paused ? " + EngineManager.isPaused());

      ArenaManager am = ArenaManager.get();

      if (am == null) return;

      GUILayout.Label("status : " + am.getState().ToString());

      GUILayout.Label("  menu ? " + am.isArenaStateMenu());
      GUILayout.Label("  live ? " + am.isArenaStateLive());
      GUILayout.Label("  end ? " + am.isArenaStateEnd());

      GUILayout.Label("arena manager has " + am.arenaObjects.Count + " objects");
      for (int i = 0; i < am.arenaObjects.Count; i++)
      {
        GUILayout.Label("  #" + i + " " + am.arenaObjects[i]);
      }

    }

  }

}
