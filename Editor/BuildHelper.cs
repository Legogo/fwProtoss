using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;
using Object = UnityEngine.Object;

/// <summary>
/// 
/// build by script
/// https://docs.unity3d.com/ScriptReference/BuildPipeline.BuildPlayer.html
/// 
/// menu item shortcuts
/// https://docs.unity3d.com/ScriptReference/MenuItem.html
/// 
/// clear console
/// https://answers.unity.com/questions/707636/clear-console-window.html
/// </summary>

abstract public class BuildHelper
{
  static BuildPlayerOptions buildPlayerOptions;

  IEnumerator process;

  DataBuildSettings data;
  bool auto_run = false;

  public BuildHelper(bool autorun = false, DataBuildSettings data = null)
  {
    //update data
    if (data == null) data = getScriptableDataBuildSettings();
    if (data != null) SettingsManager.applySettings(data);

    Debug.Log("starting build process");

    start_build();

    auto_run = autorun;
  }

  void start_build()
  {
    EditorApplication.update += update_check_process;

    process = process_setup_build();
  }

  void update_check_process()
  {

    if (!process.MoveNext())
    {
      EditorApplication.update -= update_check_process;

      Debug.Log("BuildHelper, pre build process done, start building");

      build_android();
    }

  }

  virtual protected IEnumerator process_setup_build()
  {
    yield return null;
    //...
  }

  abstract public string getBuildPathFolder();
  abstract public string getBuildName();

  protected void build_android()
  {
    if (BuildPipeline.isBuildingPlayer) return;

    buildPlayerOptions = new BuildPlayerOptions();

    VersionManager.incrementFix(); // change version

    //buildPlayerOptions.scenes = new[] { "Assets/Scene1.unity", "Assets/Scene2.unity" };
    buildPlayerOptions.scenes = getScenePaths();

    
    string path = getBuildPathFolder();
    if (!path.EndsWith("/")) path += "/";

    path += getBuildName();
    path += "_" + VersionManager.getFormatedVersion('-');
    path += "_" + PlayerSettings.Android.bundleVersionCode;
    path += "_" + getFullDate();

    // [project]/build_path/build-name_version_build-number_fulldatetime

    if (!path.EndsWith(".apk")) path += ".apk";

    Debug.Log("BuildHelper, saving build at : " + path);

    buildPlayerOptions.locationPathName = path;
    buildPlayerOptions.target = BuildTarget.Android;

    buildPlayerOptions.options |= BuildOptions.Development;
    if(auto_run) buildPlayerOptions.options |= BuildOptions.AutoRunPlayer;
    
    BuildPipeline.BuildPlayer(buildPlayerOptions);
  }

  /// <summary>
  /// yyyy-mm-dd_hh:mm
  /// </summary>
  static protected string getFullDate()
  {
    DateTime dt = DateTime.Now;
    return dt.Year + "-" + dt.Month + "-" + dt.Day + "_" + dt.Hour + "-" + dt.Minute;
  }

  static protected string[] getScenePaths()
  {
    
    List<string> sceneNames = new List<string>();
    int count = SceneManager.sceneCountInBuildSettings;

    Debug.Log("BuildHelper, scenes count : " + count);

    EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

    for (int i = 0; i < scenes.Length; i++)
    {
      sceneNames.Add(scenes[i].path);
      Debug.Log("  --> " + i + " , adding " + scenes[i].path);
    }

    return sceneNames.ToArray();
  }

  static public DataBuildSettings getScriptableDataBuildSettings()
  {
    string[] all = AssetDatabase.FindAssets("t:DataBuildSettings");
    for (int i = 0; i < all.Length; i++)
    {
      Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(DataBuildSettings));
      DataBuildSettings data = obj as DataBuildSettings;
      if(data != null) return data;
    }
    return null;
  }

  /*
  [MenuItem("Build/Build Android #&b")]
  public static void menu_build_android()
  {
    build_android(false);
  }

  [MenuItem("Build/Build&Run Android #b")]
  public static void menu_build_run_android()
  {
    new BuildHelper();
    build_android(true);
  }
  */

}