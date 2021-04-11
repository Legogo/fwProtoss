using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace builder
{

  public class BuildHalperWindow : EditorWindow
  {
    [MenuItem("Build/viewer")]
    static void init()
    {
      EditorWindow.GetWindow(typeof(BuildHalperWindow));
    }

    private void OnGUI()
    {

      GUILayout.Label("Build halper", HalperGuiStyle.getWinTitle());

      DataBuildSettingsBridge bridge = BuildHelperBase.getScriptableDataBuildSettings();
      DataBuildSettingProfile cur = bridge.getPlatformProfil();

      GUILayout.Label("platform : " + cur.getPlatformTarget());

      GUILayout.BeginHorizontal();
      GUILayout.Label("zip name :");
      GUILayout.TextArea(cur.getZipName());
      GUILayout.EndHorizontal();
    }

  }

}