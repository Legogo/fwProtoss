using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Object = UnityEngine.Object;
using UnityEditor.Build.Reporting;

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

  DataBuildSettings data = null;
  bool auto_run = false;
  bool version_increment = false;

  public BuildHelper(bool autorun = false, bool incVersion = true, DataBuildSettings paramData = null)
  {
    //update data
    if (paramData != null) data = paramData;
    else data = GlobalSettingsBuild.getScriptableDataBuildSettings();

    if (data != null) GlobalSettingsBuild.applySettings(data);

    Debug.Log("starting build process");

    start_build();

    auto_run = autorun;
    version_increment = incVersion;
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

      build_android(version_increment);
    }

  }

  virtual protected IEnumerator process_setup_build()
  {
    yield return null;
    //...
  }

  protected void build_android(bool incVersion)
  {
    if (BuildPipeline.isBuildingPlayer) return;

    buildPlayerOptions = new BuildPlayerOptions();

    if (incVersion) VersionManager.incrementFix();
    else VersionManager.incrementBuildNumber();

    //buildPlayerOptions.scenes = new[] { "Assets/Scene1.unity", "Assets/Scene2.unity" };
    buildPlayerOptions.scenes = getScenePaths();

    
    string path = getBuildPathFolder();
    if (!path.EndsWith("/")) path += "/";

    path += getBuildName();
    path += "_" + VersionManager.getFormatedVersion('-');
    path += "_" + PlayerSettings.Android.bundleVersionCode;
    path += "_" + HalperNatives.getFullDate();

    // [project]/build_path/build-name_version_build-number_fulldatetime

    if (!path.EndsWith(".apk")) path += ".apk";

    Debug.Log("BuildHelper, saving build at : " + path);

    buildPlayerOptions.locationPathName = path;
    
    //will setup android or ios based on unity build settings target platform
    buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;

    buildPlayerOptions.options |= BuildOptions.Development;
    if(auto_run) buildPlayerOptions.options |= BuildOptions.AutoRunPlayer;

    //BuildPipeline.BuildPlayer(buildPlayerOptions);
    
    BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
    BuildSummary summary = report.summary;

    if (summary.result == BuildResult.Succeeded)
    {
      onSuccess(summary);
    }

    if (summary.result == BuildResult.Failed)
    {
      Debug.Log("Build failed");
    }
  }

  protected void onSuccess(BuildSummary summary) {

    Debug.Log("Build succeeded: " + summary.totalSize + " bytes");

    //DataBuildSettings data = SettingsManager.getScriptableDataBuildSettings();

    if (data.openFolderOnBuildSuccess)
    {
      Debug.Log("opening build folder ...");
      openBuildFolder();
    }
  }

  protected void openBuildFolder()
  {
    string path = getBuildPathFolder();
    HalperNatives.os_openFolder(path);
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

  abstract public string getBuildPathFolder();
  abstract public string getBuildName();

}




/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Builder : BuildHelper {

  public Builder(bool run, bool inc) : base(run, inc)
  { }

  [MenuItem("Build/Build&Run (no-increment) %&x")]
  public static void menu_build_android() { new Builder(true, false); }

  [MenuItem("Build/Build&Run (increment) %&c")]
  public static void menu_build_run_android() { new Builder(true, true); }

  public override string getBuildName()
  {
    return "workingTitle";
  }

  public override string getBuildPathFolder()
  {
    return "Build/";
  }

}
*/
