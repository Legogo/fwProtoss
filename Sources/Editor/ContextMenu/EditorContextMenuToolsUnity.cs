using UnityEditor;

/// <summary>
/// some various context menu related to Unity
/// </summary>

public class EditorContextMenuToolsUnity
{
  [MenuItem("Tools/Clear PlayerPrefs")]
  public static void ctxmClearPPrefs()
  {
    HalperUnity.clearPlayerPrefs();
  }

  [MenuItem("Tools/open persistant data path")]
  static public void osOpenDataPathFolder()
  {
    HalperNatives.os_openFolder(HalperNatives.getDataPath());
  }

}
