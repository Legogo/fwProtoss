using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
/// buils size
/// 
/// https://stackoverflow.com/questions/28100362/how-to-reduce-the-size-of-an-apk-file-in-unity
/// https://docs.unity3d.com/Manual/iphone-playerSizeOptimization.html
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

namespace fwp.build
{
  using UnityEditor.Build.Reporting;
  using UnityEditor;

  public class BuildHelperBase
  {
    static BuildPlayerOptions buildPlayerOptions;

    IEnumerator process;

    ScriptableBuildSettingsData data = null;
    bool auto_run = false;
    bool version_increment = false;
    bool open_on_sucess = false;

    public BuildHelperBase(bool autorun = false, bool incVersion = true, bool openFolderOnSucess = false, ScriptableBuildSettingsData paramData = null)
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

      if (incVersion) DataBuildSettingsGui.incFix();
      else VersionManager.incrementBuildNumber();

      //buildPlayerOptions.scenes = new[] { "Assets/Scene1.unity", "Assets/Scene2.unity" };
      buildPlayerOptions.scenes = getScenePaths();

    
      string path = getBuildPathFolder();
      if (!path.EndsWith("/")) path += "/";

      path += getBuildName();
      path += "_" + VersionManager.getFormatedVersion('-');
      path += "_" + PlayerSettings.Android.bundleVersionCode;
      path += "_" + HalperTime.getFullDate();

      // [project]/build_path/build-name_version_build-number_fulldatetime

      if (!path.EndsWith(".apk")) path += ".apk";

      Debug.Log("BuildHelper, saving build at : " + path);

      buildPlayerOptions.locationPathName = path;
    
      //will setup android or ios based on unity build settings target platform
      buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;

      if (data.developementBuild)
      {
        buildPlayerOptions.options |= BuildOptions.Development;
      }

      if (auto_run)
      {
        buildPlayerOptions.options |= BuildOptions.AutoRunPlayer;
      }

      //BuildPipeline.BuildPlayer(buildPlayerOptions);

      // https://docs.unity3d.com/ScriptReference/Build.Reporting.BuildSummary.html
    
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

      ulong bytes = summary.totalSize;
      ulong byteToMo = 1048576;

      int size = (int)(bytes / byteToMo);

      Debug.Log("Build finished");
      Debug.Log("  L version : <b>" + VersionManager.getFormatedVersion()+"</b>");
      Debug.Log("  L result : " + summary.result+" | warnings : "+summary.totalWarnings+" | errors "+summary.totalErrors);
      Debug.Log("  L platform : <b>"+summary.platform+"</b>");
      Debug.Log("  L build time : " + summary.totalTime);

      switch (summary.result)
      {
        case BuildResult.Succeeded:

          Debug.Log("  L byte size : " + summary.totalSize + " bytes");
          Debug.Log("  L ~ size : " + size + " Mo");
          Debug.Log("  L path : " + summary.outputPath);

          //DataBuildSettings data = SettingsManager.getScriptableDataBuildSettings();

          if (data.openFolderOnBuildSuccess)
          {
            Debug.Log("opening build folder ...");
            openBuildFolder();
          }
          break;
        default:
          Debug.Log("Build failed: " + summary.result);
          break;
      }

    }

    protected void openBuildFolder()
    {
      string path = getBuildPathFolder();
      HalperNatives.os_openFolder(path);
    }

    protected string getBuildName()
    {
      ScriptableBuildSettingsData data = GlobalSettingsBuild.getScriptableDataBuildSettings();
      return data.build_prefix;
    }

    protected string getBuildPathFolder()
    {
      ScriptableBuildSettingsData data = GlobalSettingsBuild.getScriptableDataBuildSettings();
      if (data == null)
      {
        Debug.LogError("no data ?");
        return "";
      }
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

    static public DataBuildSettings getScriptableDataBuildSettings()
    {
      string[] all = AssetDatabase.FindAssets("t:DataBuildSettings");
      for (int i = 0; i < all.Length; i++)
      {
        Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(DataBuildSettings));
        DataBuildSettings data = obj as DataBuildSettings;
        if (data != null) return data;
      }
      return null;
    }

    static public DataBuildSettingProfile getActiveProfile()
    {
      return getScriptableDataBuildSettings().activeProfile;
    }

    [MenuItem("Build/Apply settings")]
    static public void applySettings()
    {
      DataBuildSettingProfile data = getActiveProfile();
      applySettings(data);
    }

    static public void applySettings(DataBuildSettingProfile data)
    {
      Debug.Log("applying profile : " + data.name + " ...");

      data.apply(); // apply version

      PlayerSettings.productName = data.product_name;
      PlayerSettings.companyName = data.compagny_name;

      PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, data.package_name);
      PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, data.package_name);

      PlayerSettings.defaultInterfaceOrientation = data.orientation_default;

      Texture2D[] icons = new Texture2D[1];

#if UNITY_IPHONE
    icons[0] = data.icon_ios;
#elif UNITY_ANDROID
    icons[0] = data.icon_android;
#endif

      PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, icons);

      Debug.Log("  L updated icons");

      PlayerSettings.SplashScreen.show = false;
      //PlayerSettings.SplashScreen.background = data.splashscreen;
      //PlayerSettings.SplashScreenLogo.unityLogo = data.splashscreen;
      //PlayerSettings.SplashScreenLogo.unityLogo = data.splashscreen;
      Debug.Log("  L updated splash");

      EditorUserBuildSettings.development = data.developementBuild;

      //android specific
      PlayerSettings.Android.minSdkVersion = data.minSdk;
      Debug.Log("  L updated android stuff");

      //ios specific
      PlayerSettings.iOS.targetDevice = data.target_device;
      PlayerSettings.iOS.targetOSVersionString = data.iOSVersion;
      Debug.Log("  L updated ios stuff");
    }

    [MenuItem("Build/Build n Open")]
    public static void menu_build_open() { new BuildHelperBase(false, false, true); }
  
    [MenuItem("Build/Build n Run (no-increment) %&x")]
    public static void menu_build_android() { new BuildHelperBase(true, false); }

    [MenuItem("Build/Build n Run (increment) %&c")]
    public static void menu_build_run_android() { new BuildHelperBase(true, true); }
    
  }

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
