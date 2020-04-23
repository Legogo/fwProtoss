
/// <summary>
/// 
/// https://docs.unity3d.com/ScriptReference/EditorBuildSettingsScene.html
/// 
/// </summary>
namespace fwp.build
{
  using System.Collections.Generic;
  using UnityEngine;
  using UnityEditor;

  [CreateAssetMenu(menuName = "protoss/create DataBuildSettingProfilScenes", order = 100)]
  public class DataBuildSettingProfilScenes : ScriptableObject
  {
    public string[] build_settings_scenes_paths;

#if UNITY_EDITOR
    [ContextMenu("apply")]
    public DataBuildSettingProfilScenes apply()
    {
      List<EditorBuildSettingsScene> tmp = new List<EditorBuildSettingsScene>();

      foreach (string path in build_settings_scenes_paths)
      {
        tmp.Add(new EditorBuildSettingsScene(path, true));
      }

      EditorBuildSettings.scenes = tmp.ToArray();

      return this;
    }

    [ContextMenu("record")]
    public DataBuildSettingProfilScenes record()
    {
      List<string> tmp = new List<string>();

      EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
      foreach (EditorBuildSettingsScene sc in scenes)
      {
        Debug.Log(sc.path);
        tmp.Add(sc.path);
      }
      build_settings_scenes_paths = tmp.ToArray();

      EditorUtility.SetDirty(this);

      return this;
    }
    public static void injectAll()
    {
      DataBuildSettingProfilScenes scenes = HalperScriptables.getScriptableObjectInEditor<DataBuildSettingProfilScenes>("game_release");
      scenes.apply();
      Debug.Log("re-applied all scenes from scriptable " + scenes.name, scenes);
    }

#endif


  }
}