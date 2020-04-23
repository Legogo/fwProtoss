﻿
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
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;
  using UnityEngine.SceneManagement;
  using System.IO;

#if UNITY_EDITOR
  using UnityEditor.Build.Reporting;
  using UnityEditor;
#endif

  public class BuildHelperBase
  {
#if UNITY_EDITOR
    BuildPlayerOptions buildPlayerOptions;

    IEnumerator preProc = null;
    IEnumerator buildProc = null;

    //float time_at_process = 0f;

    DataBuildSettingsBridge data = null;
    bool auto_run = false;
    bool version_increment = false;
    bool open_on_sucess = false;

    string outputPath = "";

    public BuildHelperBase(bool autorun = false, bool incVersion = true, bool openFolderOnSucess = false, DataBuildSettingsBridge paramData = null)
    {
      //update data
      if (paramData != null) data = paramData;
      else data = getScriptableDataBuildSettings();

      //if (data != null) applySettings(data.activeProfile);

      Debug.Log("starting build process");
      
      this.auto_run = autorun;
      this.version_increment = incVersion;
      this.open_on_sucess = openFolderOnSucess;

      preProc = preBuildProcess();
      
      EditorApplication.update += update_check_process;
    }
    
    /// <summary>
    /// update in editor
    /// </summary>
    void update_check_process()
    {
      //Debug.Log("it " + Time.realtimeSinceStartup);

      if(preProc != null)
      {
        if(!preProc.MoveNext())
        {
          preProc = null;

          Debug.Log("pre proc done");

          buildProc = buildProcess();
        }
        return;
      }
      
      if(buildProc != null)
      {
        if(!buildProc.MoveNext())
        {
          Debug.Log("build proc done");

          buildProc = null;
          EditorApplication.update -= update_check_process;
        }
        return;
      }
      
    }
    
    /// <summary>
    /// whatever is needed to do before building
    /// </summary>
    /// <returns></returns>
    virtual protected IEnumerator preBuildProcess()
    {
      yield return null;
      //...
    }

    protected IEnumerator buildProcess()
    {
      Debug.Log("BuildHelper, prep building ...");
      build_prep();

      //wait 5 secondes
      float startupTime = Time.realtimeSinceStartup;
      float curTime = Time.realtimeSinceStartup;
      int psec = Mathf.FloorToInt(startupTime);

      float waitTime = 2f;
      Debug.Log("BuildHelper, waiting "+waitTime+" secondes ...");

      while (curTime - startupTime < waitTime)
      {
        curTime = Time.realtimeSinceStartup;
        
        //Debug.Log(curTime);
        yield return null;
      }

      //curTime = Time.realtimeSinceStartup;
      //Debug.Log(curTime);

      Debug.Log("BuildHelper, building ...");
      Debug.Log(buildPlayerOptions.locationPathName);

      yield return null;

      build_app();

    }

    protected void build_prep()
    {

      if (BuildPipeline.isBuildingPlayer) return;

      Debug.Log("now building app ; inc version ? "+ version_increment);

      buildPlayerOptions = new BuildPlayerOptions();

      DataBuildSettingProfile profile = data.getPlatformProfil();

      if(profile == null)
      {
        Debug.LogError("no profile for current platform ?");
        return;
      }

      profile.apply();

      DataBuildSettingProfileAndroid pAndroid = profile as DataBuildSettingProfileAndroid;
      DataBuildSettingProfileIos pIos = profile as DataBuildSettingProfileIos;
      DataBuildSettingProfileWindows pWindows = profile as DataBuildSettingProfileWindows;
      DataBuildSettingProfileSwitch pSwitch = profile as DataBuildSettingProfileSwitch;

      //this will apply
      if (version_increment)
      {
        DataBuildSettingsBridgeEditor.incFix();
        VersionManager.incrementBuildNumber();
      }
      
      //apply everything (after inc)
      applySettings(profile);

      //buildPlayerOptions.scenes = new[] { "Assets/Scene1.unity", "Assets/Scene2.unity" };
      buildPlayerOptions.scenes = getScenePaths();

      // === CREATE SOLVED BUILD PATH

      string absPath = profile.getBasePath();

      bool pathExists = Directory.Exists(absPath);

      if (!pathExists)
      {
        Directory.CreateDirectory(absPath);
        Debug.Log("  ... created : "+absPath);
      }

      // === INJECTING SOLVED PATH TO BUILD SETTINGS

      outputPath = absPath;

      //[project]_[version].[ext]
      absPath = Path.Combine(absPath, profile.getBuildFullName(true));

      Debug.Log("BuildHelper, saving build at : " + absPath);
      buildPlayerOptions.locationPathName = absPath;
      
      //will setup android or ios based on unity build settings target platform
      buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;

      if (profile.developement_build)
      {
        buildPlayerOptions.options |= BuildOptions.Development;
      }

      if (auto_run)
      {
        buildPlayerOptions.options |= BuildOptions.AutoRunPlayer;
      }

      //BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    protected void build_app()
    {
      // https://docs.unity3d.com/ScriptReference/Build.Reporting.BuildSummary.html

      DataBuildSettingProfile profile = data.getPlatformProfil();

      BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
      BuildSummary summary = report.summary;

      if (summary.result == BuildResult.Succeeded)
      {
        onSuccess(summary, (profile.openFolderOnBuildSuccess || open_on_sucess));
      }

      if (summary.result == BuildResult.Failed)
      {
        Debug.LogError("Build failed");
      }
    }
  
    protected void onSuccess(BuildSummary summary, bool openFolder = false) {

      ulong bytes = summary.totalSize;
      ulong byteToMo = 1048576;

      int size = (int)(bytes / byteToMo);

      bool success = summary.totalErrors <= 0;

      Debug.Log("Build finished");
      Debug.Log("  L version : <b>" + VersionManager.getFormatedVersion()+"</b>");
      Debug.Log("  L result : summary says " + summary.result+" ( success ? "+ success +" ) | warnings : "+summary.totalWarnings+" | errors "+summary.totalErrors);
      Debug.Log("  L platform : <b>"+summary.platform+"</b>");
      Debug.Log("  L build time : " + summary.totalTime);

      switch (summary.result)
      {
        case BuildResult.Succeeded:

          Debug.Log("  L byte size : " + summary.totalSize + " bytes");
          Debug.Log("  L ~ size : " + size + " Mo");
          Debug.Log("  L path : " + summary.outputPath);

          //DataBuildSettings data = SettingsManager.getScriptableDataBuildSettings();

          if (openFolder)
          {
            Debug.Log("opening build folder ...");
            //openBuildFolder(summary.outputPath);
            openBuildFolder(outputPath);
          }
          break;
        default:
          Debug.LogError("Build failed: " + summary.result);
          break;
      }

    }

    protected void openBuildFolder(string path)
    {
      //DataBuildSettingProfile profile = getActiveProfile();
      //string path = profile.getBasePath();
      Debug.Log("opening folder : " + path);
      HalperNatives.os_openFolder(path);
    }

    protected string getBuildName()
    {
      DataBuildSettingsBridge data = getScriptableDataBuildSettings();
      return data.getPlatformProfil().build_prefix;
    }
    static protected string[] getScenePaths()
    {
    
      List<string> sceneNames = new List<string>();
      int count = SceneManager.sceneCountInBuildSettings;

      Debug.Log("BuildHelper, adding "+count+" scenes to list");

      EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

      for (int i = 0; i < scenes.Length; i++)
      {
        sceneNames.Add(scenes[i].path);
        //Debug.Log("  --> " + i + " , adding " + scenes[i].path);
      }

      return sceneNames.ToArray();
    }

    static public DataBuildSettingsBridge getScriptableDataBuildSettings()
    {
      string[] all = AssetDatabase.FindAssets("t:DataBuildSettingsBridge");

      if (all.Length > 0)
      {
        for (int i = 0; i < all.Length; i++)
        {
          Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(DataBuildSettingsBridge));
          DataBuildSettingsBridge data = obj as DataBuildSettingsBridge;
          if (data != null) return data;
        }
      }
      else Debug.LogWarning("no objects returned by AssetDatabase for type : DataBuildSettingsBridge");

      Debug.LogError("could not find object of type : DataBuildSettingsBridge");
      return null;
    }

    static public DataBuildSettingProfile getActiveProfile()
    {
      return getScriptableDataBuildSettings().getPlatformProfil();
    }

    [MenuItem("Build/Apply platform settings")]
    static public void applySettings()
    {
      DataBuildSettingProfile data = getActiveProfile();
      applySettings(data);
    }

    static public void applySettings(DataBuildSettingProfile profil)
    {
      if(profil == null)
      {
        Debug.LogError("no profil ?");
        return;
      }

      BuildTarget bt = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

      //if (bt == null) Debug.LogError("no build target ?");

      Debug.Log("applying profile : <b>" + profil.name + "</b> | current platform ? "+bt);
      
      Debug.Log("~Globals~");

      profil.version.applyCurrent(); // apply version
      
      PlayerSettings.companyName = profil.compagny_name;
      Debug.Log("  L companyName : " + PlayerSettings.companyName);

      //α,β,Ω
      string productName = profil.getProductName();
      if(profil.phase != BuildPhase.none && profil.phase != BuildPhase.Ω)
      {
        productName += "("+profil.phase+")";
      }
      PlayerSettings.productName = productName;
      Debug.Log("  L productName : " + PlayerSettings.productName);
      
      //TODO mobile
      //PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, profil.package_name);
      //PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, profil.package_name);

      Debug.Log("~Systems~");

      DataBuildSettingProfileMobile pMobile = profil as DataBuildSettingProfileMobile;
      if(pMobile != null)
      {
        PlayerSettings.defaultInterfaceOrientation = pMobile.orientation_default;
      }
      
      Texture2D[] icons = new Texture2D[1];
      icons[0] = profil.icon;
      
      PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, icons);

      Debug.Log("  L updated icons");

      PlayerSettings.SplashScreen.show = false;
      //PlayerSettings.SplashScreen.background = data.splashscreen;
      //PlayerSettings.SplashScreenLogo.unityLogo = data.splashscreen;
      //PlayerSettings.SplashScreenLogo.unityLogo = data.splashscreen;
      Debug.Log("  L updated splash");

      //dev build
#if UNITY_EDITOR
      UnityEditor.EditorUserBuildSettings.development = profil.developement_build;
      
      DataBuildSettingProfileSwitch pSwitch = profil as DataBuildSettingProfileSwitch;
      if(pSwitch != null)
      {
        EditorUserBuildSettings.switchCreateRomFile = pSwitch.build_rom;
      }

#endif

      //android specific
      DataBuildSettingProfileAndroid pAndroid = profil as DataBuildSettingProfileAndroid;
      if(pAndroid != null)
      {
        PlayerSettings.Android.minSdkVersion = pAndroid.minSdk;
        Debug.Log("  L updated android stuff");
      }

      DataBuildSettingProfileIos pIos = profil as DataBuildSettingProfileIos;
      if (pIos != null)
      {
        //ios specific
        PlayerSettings.iOS.targetDevice = pIos.target_device;
        PlayerSettings.iOS.targetOSVersionString = pIos.iOSVersion;
        Debug.Log("  L updated ios stuff");
      }

      Debug.Log("~output~");
      Debug.Log("  L base path : " + profil.getBasePath());
      Debug.Log("  L build name : " + profil.getBuildFullName(true));

    }

    [MenuItem("Build/Build n Open platform (no-increment)")]
    public static void menu_build_open() { new BuildHelperBase(false, false, true); }

    [MenuItem("Build/Build n Open platform (increment)")]
    public static void menu_build_open_inc() { new BuildHelperBase(false, true, true); }

    [MenuItem("Build/Build n Run platform (no-increment) %&x")]
    public static void menu_build_run() { new BuildHelperBase(true, false); }

    [MenuItem("Build/Build n Run platform (increment) %&c")]
    public static void menu_build_run_inc() { new BuildHelperBase(true, true); }


#endif


  }

}
