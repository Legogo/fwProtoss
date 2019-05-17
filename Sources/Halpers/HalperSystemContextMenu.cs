using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

static public class HalperSystemContextMenu
{
#if UNITY_EDITOR
  [MenuItem("Tools/Clear PlayerPrefs")]
  public static void ctxmClearPPrefs()
  {
    ClearPlayerPrefs();
  }
#endif

  public static void ClearPlayerPrefs()
  {
    PlayerPrefs.DeleteAll();
    PlayerPrefs.Save();

    Debug.Log("all pprefs deleted");
  }
}
