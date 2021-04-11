using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

namespace builder
{
  [CreateAssetMenu(menuName = "builder/create DataBuildSettingsBridge", order = 100)]
  public class DataBuildSettingsBridge : ScriptableObject
  {
    public DataBuildSettingProfileWindows windows;
    public DataBuildSettingProfileAndroid android;
    public DataBuildSettingProfileIos ios;
    public DataBuildSettingProfileOsx osx;
    public DataBuildSettingProfileSwitch switcheu;

    public DataBuildSettingProfile getPlatformProfil()
    {
      BuildTarget target = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

      if (target == BuildTarget.iOS)
      {
        return ios;
      }

      if (target == BuildTarget.Android)
      {
        return android;
      }

      if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
      {
        return windows;
      }

      if (target == BuildTarget.Switch)
      {
        return switcheu;
      }

      if (target == BuildTarget.StandaloneOSX)
      {
        return osx;
      }

      Debug.LogError("not implem for " + target);

      //throw new NotImplementedException("not implem for " + target);

      return null;
    }

  }

}
