
/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

namespace fwp.build
{
  using UnityEngine;
  using UnityEditor;

  [CreateAssetMenu(menuName = "protoss/new profil ios", order = 100)]
  public class DataBuildSettingProfileIos : DataBuildSettingProfileMobile
  {

    public override string getExtension() => "";

#if UNITY_EDITOR
    [Header("SDK")]
    public iOSTargetDevice target_device;
    public string iOSVersion = "9.0";

    public override BuildTarget getPlatformTarget() => BuildTarget.iOS;

#endif
  }
}