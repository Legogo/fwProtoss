using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace fwp.build
{
  [CustomEditor(typeof(DataBuildSettings))]
  public class DataBuildSettingsGui : Editor
  {
    override public void OnInspectorGUI()
    {
      DrawDefaultInspector();

      EditorGUILayout.Separator();

      if (GUILayout.Button("apply settings"))
      {
        BuildHelperBase.applySettings();
      }

      if (GUILayout.Button("apply scenes"))
      {

      }

      if (GUILayout.Button("record scenes"))
      {

      }
    }


    [MenuItem("Version/Incr X.minor.build")]
    static public void incMajor()
    {
      DataBuildSettingProfile prof = BuildHelperBase.getActiveProfile();
      prof.version.incrementMajor();
    }

    [MenuItem("Version/Incr major.X.build")]
    static public void inMinor()
    {
      DataBuildSettingProfile prof = BuildHelperBase.getActiveProfile();
      prof.version.incrementMinor();
    }

    [MenuItem("Version/Incr major.minor.X")]
    static public void incFix()
    {
      DataBuildSettingProfile prof = BuildHelperBase.getActiveProfile();
      prof.version.incrementFix();
    }
  }

}
