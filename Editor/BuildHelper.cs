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
/// 
/// what is supposed to be version number on ios
/// https://stackoverflow.com/questions/21125159/which-ios-app-version-build-numbers-must-be-incremented-upon-app-store-release/38009895#38009895
/// 
/// 
/// The pair(Version, Build number) must be unique.
/// The sequence is valid: (1.0.1, 12) -> (1.0.1, 13) -> (1.0.2, 13) -> (1.0.2, 14) ...
/// Version(CFBundleShortVersionString) must be in ascending sequential order.
/// Build number(CFBundleVersion) must be in ascending sequential order.
/// 
/// Based on the checklist, the following (Version, Build Number) sequence is valid too.
/// Case: reuse Build Number in different release trains.
/// (1.0.0, 1) -> (1.0.0, 2) -> ... -> (1.0.0, 11) -> (1.0.1, 1) -> (1.0.1, 2)
/// 
/// </summary>

public class BuildHelper
{
  static BuildPlayerOptions buildPlayerOptions;

  IEnumerator process;

  DataBuildSettings data = null;
  bool auto_run = false;
  bool version_increment = false;
  bool open_on_sucess = false;

  public BuildHelper(bool autorun = false, bool incVersion = true, bool openFolderOnSucess = false, DataBuildSettings paramData = null)
  {
    //update data
    if (paramData != null) data = paramData;
    else data = GlobalSettingsBuild.getScriptableDataBuildSettings();

    if (data != null) GlobalSettingsBuild.applySettings(data);

    Debug.Log("starting build process");


    auto_run = autorun;
    version_increment = incVersion;
    open_on_sucess = openFolderOnSucess;

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
      onSuccess(summary, data.openFolderOnBuildSuccess || open_on_sucess);
    }

    if (summary.result == BuildResult.Failed)
    {
      Debug.Log("Build failed");
    }
  }

  protected void onSuccess(BuildSummary summary, bool openFolder = false) {

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

  protected string getBuildName()
  {
    DataBuildSettings data = GlobalSettingsBuild.getScriptableDataBuildSettings();
    return data.build_prefix;
  }

  protected string getBuildPathFolder()
  {
    DataBuildSettings data = GlobalSettingsBuild.getScriptableDataBuildSettings();
    return data.build_path;
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

#if UNITY_EDITOR

  [MenuItem("Build/Build n Open")]
  public static void menu_build_open() { new BuildHelper(false, false, true); }
  
  [MenuItem("Build/Build n Run (no-increment) %&x")]
  public static void menu_build_android() { new BuildHelper(true, false); }

  [MenuItem("Build/Build n Run (increment) %&c")]
  public static void menu_build_run_android() { new BuildHelper(true, true); }

#endif

}




/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Builder : BuildHelper {

  public Builder(bool run, bool inc) : base(run, inc)
  { }

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
