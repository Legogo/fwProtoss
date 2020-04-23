
/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>
namespace fwp.build
{
  using UnityEngine;
  using UnityEditor;

  [CreateAssetMenu(menuName = "protoss/new profil android", order = 100)]
  public class DataBuildSettingProfileAndroid : DataBuildSettingProfileMobile
  {
    public override string getExtension() => "apk";

#if UNITY_EDITOR
    [Header("SDK")]
    public AndroidSdkVersions minSdk = AndroidSdkVersions.AndroidApiLevel21; // android 5.0

    public override BuildTarget getPlatformTarget() => BuildTarget.Android;
#endif

  }

}
