
/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

namespace fwp.build
{
  using UnityEngine;
  using UnityEditor;

  abstract public class DataBuildSettingProfileMobile : DataBuildSettingProfile
  {
    [Header("mobile section")]

    public string package_name = "com.*.*";

    public bool use_joystick_visualisation = true;

#if UNITY_EDITOR
    public UIOrientation orientation_default = UIOrientation.Portrait;
#endif

  }

}