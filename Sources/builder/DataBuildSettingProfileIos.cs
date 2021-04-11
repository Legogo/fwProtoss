using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

namespace builder
{
  [CreateAssetMenu(menuName = "builder/new profil ios", order = 100)]
  public class DataBuildSettingProfileIos : DataBuildSettingProfileMobile
  {

#if UNITY_EDITOR
    [Header("SDK")]
    public iOSTargetDevice target_device;
    public string iOSVersion = "9.0";
#endif

    public override string getExtension() => "";

    public override BuildTarget getPlatformTarget() => BuildTarget.iOS;
  }

}
