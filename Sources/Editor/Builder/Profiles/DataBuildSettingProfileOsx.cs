
/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>
namespace fwp.build
{
  using UnityEngine;
  using UnityEditor;

  [CreateAssetMenu(menuName = "protoss/new profil osx", order = 100)]
  public class DataBuildSettingProfileOsx : DataBuildSettingProfile
  {
    public override string getExtension() => "app";

#if UNITY_EDITOR
    public override BuildTarget getPlatformTarget() => BuildTarget.StandaloneOSX;
#endif

  }

}