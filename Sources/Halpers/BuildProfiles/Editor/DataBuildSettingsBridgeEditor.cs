using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace fwp.build
{
#if UNITY_EDITOR
  [CustomEditor(typeof(DataBuildSettingsBridge))]
  public class DataBuildSettingsBridgeEditor : Editor
  {
    override public void OnInspectorGUI()
    {
      DrawDefaultInspector();

      EditorGUILayout.Separator();

      DataBuildSettingsBridge handle = (DataBuildSettingsBridge)target;

      if (GUILayout.Button("apply all settings"))
      {
        handle.activeScenes.apply();
        BuildHelperBase.applySettings();
      }

      EditorGUILayout.Separator();

      if(handle.profiles != null)
      {
        for (int i = 0; i < handle.profiles.Length; i++)
        {
          if (GUILayout.Button("apply " + handle.profiles[i].name))
          {
            handle.profiles[i].apply();
          }
        }
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
#endif
}
