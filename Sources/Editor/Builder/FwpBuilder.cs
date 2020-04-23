﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
using UnityEditor.Build.Reporting;
using UnityEditor;
#endif

public class FwpBuilder
{
  /*
#if UNITY_EDITOR
  string buildPath = "";
  string buildName = "";

  BuildPlayerOptions buildOptions;

  float build_startTime = 0f;

  IEnumerator flowProc = null;

  IEnumerator preProc = null;
  IEnumerator buildProc = null;
  IEnumerator postProc = null;

  //float time_at_process = 0f;

  //BuildTarget targetPlatform;
  bool auto_run = false;
  bool version_increment = false;
  bool open_on_sucess = false;
  bool dev_build = false;

  public Action onDone;

  public FwpBuilder(bool devBuild = false, bool autorun = false, bool incVersion = true, bool openFolderOnSucess = false)
  {
    //this.targetPlatform = targetPlatform;
    this.dev_build = devBuild;
    this.auto_run = autorun;
    this.version_increment = incVersion;
    this.open_on_sucess = openFolderOnSucess;

    build_startTime = Time.time;

    buildOptions = new BuildPlayerOptions();

    Debug.Log("starting build process at "+build_startTime);

    if (BuildPipeline.isBuildingPlayer)
    {
      Debug.LogWarning("already buildling ?");
      return;
    }

    flowProc = buildFlow();
    
    //subscribe to update
    EditorApplication.update += update_process;
  }

  void update_process()
  {
    if (flowProc != null)
    {
      if(!flowProc.MoveNext())
      {
        //Debug.Log("unsub from update");
        flowProc = null;
        EditorApplication.update -= update_process;
      }
    }
  }

  protected IEnumerator buildFlow()
  {

    //do stuff before building ...
    preProc = preBuildProcess();
    while (preProc.MoveNext()) yield return null;

    buildProc = buildProcess();
    while (buildProc.MoveNext()) yield return null;

    postProc = postProcess();
    while (postProc.MoveNext()) yield return null;
    Debug.Log("<b>post process</b> <color=green>completed</color>");

    //Debug.Log("building flow is done");
  }

  /// <summary>
  /// whatever is needed to do before building
  /// </summary>
  virtual protected IEnumerator preBuildProcess()
  {
    yield return null;

    //this will apply
    if (version_increment)
    {
      Debug.Log("  flag as : increment version from "+VersionManager.getFormatedVersion());

      string newVersion = incrementVersion();
      
      Debug.Log("  L new build version : "+newVersion);
    }

    yield return null;

    //wait 5 secondes
    float startupTime = Time.realtimeSinceStartup;
    float curTime = Time.realtimeSinceStartup;
    int psec = Mathf.FloorToInt(startupTime);

    float waitTime = 2f;
    Debug.Log("BuildHelper, waiting " + waitTime + " secondes ...");

    while (curTime - startupTime < waitTime)
    {
      curTime = Time.realtimeSinceStartup;

      //Debug.Log(curTime);
      yield return null;
    }

    if (dev_build)
    {
      Debug.Log("  flag as : development build");
      buildOptions.options |= BuildOptions.Development;
    }

    if (auto_run)
    {
      Debug.Log("  flag as : autorun");
      buildOptions.options |= BuildOptions.AutoRunPlayer;
    }

    buildOptions.scenes = getScenePaths();

    //will setup android or ios based on unity build settings target platform
    buildOptions.target = EditorUserBuildSettings.activeBuildTarget;
    //buildOptions.target = targetPlatform;

    solveBuildName(PlayerSettings.productName);
    solveBuildPath();

    string outputPath = Path.Combine(buildPath, buildName);
    buildOptions.locationPathName = outputPath;
    Debug.Log("  L output path is : " + buildOptions.locationPathName);

    Debug.Log("<b>pre process</b> <color=green>completed</color>");
  }

  virtual protected string incrementVersion()
  {
    return fwp.build.DataBuildSettingsBridgeEditor.incFix();
  }

  void solveBuildPath()
  {
    buildPath = getBuildsFolderPath(); // path/to/build/

    buildPath = Path.Combine(buildPath, buildName); // Build/chose_v

    buildOptions.locationPathName = Path.Combine(buildPath, profile;

    Debug.Log("BuildHelper, building ... "+ buildOptions.locationPathName);
  }

  void solveBuildName(string baseName)
  {
    buildName = baseName;

    //flag for dev or release
    if (dev_build) buildName += "_dev";
    else buildName += "_release";

    //add version
    buildName += "_" + VersionManager.getFormatedVersion();

    Debug.Log("  L <b>build name</b> : " + buildName);
  }

  protected IEnumerator buildProcess()
  {
    yield return null;
    yield return null;
    yield return null;

    Debug.Log(" ... NOW building ... ");

    yield return null;
    yield return null;
    yield return null;

    //actual building call
    BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

    // https://docs.unity3d.com/ScriptReference/Build.Reporting.BuildSummary.html
    BuildSummary summary = report.summary;

    if (summary.result == BuildResult.Succeeded)
    {
      onSuccess(summary, open_on_sucess);
    }

    if (summary.result == BuildResult.Failed)
    {
      Debug.LogError("Build failed");
    }
  }

  protected void onSuccess(BuildSummary summary, bool openFolder = false)
  {

    ulong bytes = summary.totalSize;
    ulong byteToMo = 1048576;

    int size = (int)(bytes / byteToMo);

    Debug.Log("Build process summary : ");
    Debug.Log("  L version : <b>" + VersionManager.getFormatedVersion() + "</b>");
    Debug.Log("  L result : " + summary.result + " | warnings : " + summary.totalWarnings + " | errors " + summary.totalErrors);
    Debug.Log("  L platform : <b>" + summary.platform + "</b>");
    Debug.Log("  L build time : " + summary.totalTime);

    switch (summary.result)
    {
      case BuildResult.Succeeded:

        Debug.Log("  L byte size : " + summary.totalSize + " bytes");
        Debug.Log("  L ~ size : " + size + " Mo");
        Debug.Log("  L path : " + summary.outputPath);

        //DataBuildSettings data = SettingsManager.getScriptableDataBuildSettings();

        Debug.Log("<b>build process</b> <color=green>completed</color>");

        if (openFolder)
        {
          Debug.Log("opening build folder ...");
          openBuildsFolder();
        }
        break;
      default:

        Debug.LogError("Build failed: " + summary.result);

        Debug.Log("<b>build process</b> <color=red>failed</color>");

        break;
    }

  }

  virtual protected IEnumerator postProcess()
  {
    yield return null;

  }

  public BuildTarget getMacOsBuildTarget()
  {

#if UNITY_2017_1_OR_NEWER
    return BuildTarget.StandaloneOSX;
#else
    return BuildTarget.StandaloneOSXIntel64; // always intel ?
#endif

  }

  /// <summary>
  /// generate an array of all defined scenes in build settings
  /// </summary>
  string[] getScenePaths()
  {

    List<string> sceneNames = new List<string>();
    int count = SceneManager.sceneCountInBuildSettings;

    UnityEngine.Debug.Log("getting scenes paths (count : " + count + ")");

    EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

    for (int i = 0; i < scenes.Length; i++)
    {
      sceneNames.Add(scenes[i].path);
      UnityEngine.Debug.Log("  #" + i + " --> " + scenes[i].path);
    }

    return sceneNames.ToArray();
  }


  [MenuItem("Build/Open builds folder")]
  static protected void openBuildsFolder()
  {
    generateBuildsFolder(); // génère build/ si il existe pas
    HalperNatives.startCmd(getBuildsFolderPath());
  }

  /// <summary>
  /// where the build will be exported to
  /// </summary>
  static public string getBuildsFolderPath()
  {
    string buildFolder = Application.dataPath; // path/to/Assets

    buildFolder = buildFolder.Substring(0, buildFolder.LastIndexOf('/')); // "/Assets" from path
    buildFolder += "/Build";

    return buildFolder;
  }

  static public string generateOutputFolder(string outputPath)
  {
    // create the root Build/ folder of not present
    if (!Directory.Exists(outputPath))
    {
      Debug.Log("  ... creating directory " + outputPath);
      Directory.CreateDirectory(outputPath);
    }

    return outputPath;
  }

  static public string generateBuildsFolder() => generateOutputFolder(getBuildsFolderPath());

#endif

  */
}
