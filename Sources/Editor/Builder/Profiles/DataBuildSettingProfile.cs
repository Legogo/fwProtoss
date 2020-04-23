
/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

namespace fwp.build
{
  using UnityEngine;
  using System.IO;

#if UNITY_EDITOR
  using UnityEditor;
#endif

  abstract public class DataBuildSettingProfile : ScriptableObject
  {
    [Header("version")]
    public DataBuildSettingVersion version;
    public BuildPhase phase = BuildPhase.α;

    [Header("identification")]
    public string compagny_name = "*";
    public string product_name = "*";
  
    [Header("file")]
    public string build_path = "builds/";

    [Tooltip("project name used to generate output file")]
    public string build_prefix = "";

    [Header("misc")]
    public bool developement_build = false;
    public bool openFolderOnBuildSuccess = false;
  
    [Header("splashscreen")]
    public Sprite splashscreen;
  
    [Header("icons")]
    public Texture2D icon;

#if UNITY_EDITOR

    private void OnValidate()
    {
      if (isSelectedPlatform()) apply();
    }

    public bool isSelectedPlatform() => getPlatformTarget() == EditorUserBuildSettings.activeBuildTarget;

    abstract public BuildTarget getPlatformTarget();

#endif

    /// <summary>
    /// path/to/build/
    /// no name
    /// </summary>
    protected string getAbsBuildFolderPath()
    {

      string baseProjectPath = Application.dataPath;
      baseProjectPath = baseProjectPath.Substring(0, baseProjectPath.LastIndexOf('/')); // remove Assets/

      //profile build suffix path 
      string buildPathFolder = build_path;
      if (!buildPathFolder.EndsWith("/")) buildPathFolder += "/";

      string absPath = Path.Combine(baseProjectPath, buildPathFolder);
      //Debug.Log("build folder => " + absPath);

      return absPath;
    }

    public string getBuildFolderName() => build_prefix + "_" + HalperTime.getFullDate();
    public string getBuildNameVersion() => build_prefix + "_" + VersionManager.getFormatedVersion('-');

    // all path by NOT build name.ext
    virtual public string getBasePath() => getAbsBuildFolderPath();

    virtual public string getBuildFullName(bool ext)
    {
      string buildName = getBuildNameVersion();
      if (ext) buildName += "." + getExtension();
      return buildName;
    }

    abstract public string getExtension();

    virtual public string getProductName() => product_name;

#if UNITY_EDITOR
    [ContextMenu("apply to player settings")]
    protected void cmApply()
    {
      apply();
    }

    virtual public void apply()
    {
      Debug.Log("applying " + name);
      //fwp.build.BuildHelperBase.applySettings(this);

      PlayerSettings.companyName = compagny_name;
      PlayerSettings.productName = getProductName();

      Texture2D[] icons = new Texture2D[1];
      icons[0] = icon;

      PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, icons);

      PlayerSettings.SplashScreen.show = false;

      EditorUserBuildSettings.development = developement_build;
    }
#endif

  }
}