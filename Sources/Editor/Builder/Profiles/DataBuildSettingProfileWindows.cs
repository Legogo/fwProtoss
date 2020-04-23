
/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>
/// 
namespace fwp.build
{
  using UnityEngine;
  using UnityEditor;

  [CreateAssetMenu(menuName = "protoss/new profil windows", order = 100)]
  public class DataBuildSettingProfileWindows : DataBuildSettingProfile
  {
    public override string getExtension() => "exe";

#if UNITY_EDITOR
    public override BuildTarget getPlatformTarget() => BuildTarget.StandaloneWindows;
#endif

  }
}