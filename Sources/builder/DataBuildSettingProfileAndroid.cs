using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// 
/// lvl 21 - android 5.0
/// lvl 23 - 6.0
/// lvl 24 - 7.0
/// lvl 26 - 8.0
/// </summary>

namespace builder
{
  [CreateAssetMenu(menuName = "builder/new profil android", order = 100)]
  public class DataBuildSettingProfileAndroid : DataBuildSettingProfileMobile
  {
    [Header("SDK")]
    public AndroidSdkVersions minSdk = AndroidSdkVersions.AndroidApiLevel21;

    public override string getExtension() => "apk";

    public override BuildTarget getPlatformTarget() => BuildTarget.Android;

  }
}
