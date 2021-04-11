﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// 
/// https://docs.unity3d.com/ScriptReference/EditorBuildSettingsScene.html
/// 
/// </summary>

[CreateAssetMenu(menuName = "builder/create DataBuildSettingProfilScenesMerger", order = 100)]
public class DataBuildSettingProfilScenesMerger : ScriptableObject
{
  public DataBuildSettingProfilScenes[] engines;
  public DataBuildSettingProfilScenes[] levels;
  
#if UNITY_EDITOR
  [ContextMenu("apply")]
  public void apply()
  {
    List<EditorBuildSettingsScene> tmp = new List<EditorBuildSettingsScene>();

    foreach(DataBuildSettingProfilScenes item in engines)
    {
      for (int i = 0; i < item.paths.Length; i++)
      {
        tmp.Add(new EditorBuildSettingsScene(item.paths[i], true));
      }
    }

    foreach (DataBuildSettingProfilScenes item in levels)
    {
      for (int i = 0; i < item.paths.Length; i++)
      {
        tmp.Add(new EditorBuildSettingsScene(item.paths[i], true));
      }
    }
    EditorBuildSettings.scenes = tmp.ToArray();
  }
  
  static public void addSceneToBuildSettings(string sceneName)
  {
    //keep existing
    List<EditorBuildSettingsScene> buildsettingScenes = new List<EditorBuildSettingsScene>();
    buildsettingScenes.AddRange(EditorBuildSettings.scenes);

    bool found = false;
    for (int j = 0; j < buildsettingScenes.Count; j++)
    {
      if (buildsettingScenes[j].path.Contains(sceneName))
      {
        found = true;
      }
    }

    if (!found)
    {
      string completePath = getSceneCompletePath(sceneName);
      buildsettingScenes.Add(new EditorBuildSettingsScene(completePath, true));

      Debug.Log("ADDED " + completePath);
    }
    
    EditorBuildSettings.scenes = buildsettingScenes.ToArray();
  }

  static public string getSceneCompletePath(string sceneName)
  {
    //AssetDatabase.FindAssets<Scene>()

    string[] all = AssetDatabase.GetAllAssetPaths();
    for (int i = 0; i < all.Length; i++)
    {
      if(all[i].Contains(sceneName+".unity"))
      {
        return all[i];
      }
    }

    return string.Empty;
  }

  public static void injectLevel(string level)
  {
    DataBuildSettingProfilScenesMerger merger = HalperScriptables.getScriptableObjectInEditor<DataBuildSettingProfilScenesMerger>("everything");

    if(merger == null)
    {
      Debug.LogError("no merger for level " + level);
      return;
    }

    if(merger.levels == null)
    {
      Debug.LogError("no levels on merger " + merger.name);
      return;
    }

    for (int i = 0; i < merger.levels.Length; i++)
    {
      if(merger.levels[i].name.Contains(level))
      {
        merger.levels[i].add();
        return;
      }
    }

    Debug.LogWarning("didn't find to inject : " + level);
  }

  public static void injectAll(string filter = "everything")
  {
    //DataBuildSettingProfilScenes scenes = HalperScriptables.getScriptableObjectInEditor<DataBuildSettingProfilScenes>("game_release");
    DataBuildSettingProfilScenesMerger merger = HalperScriptables.getScriptableObjectInEditor<DataBuildSettingProfilScenesMerger>(filter);

    merger.apply();

    Debug.Log("re-applied all scenes from scriptable " + merger.name, merger);
  }
#endif


}