using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

[CreateAssetMenu(menuName = "protoss/create DataBuildSettingProfile", order = 100)]
public class DataBuildSettingProfile : ScriptableObject
{
  [Header("version")]
  public DataBuildSettingVersion version;

  [Header("identification")]
  public string compagny_name = "*";
  public string product_name = "*";
  public string package_name = "com.*.*";

  [Header("file")]
  public string build_path = "builds/";
  public string build_prefix = "build";


#if UNITY_EDITOR
  [Header("SDK")]
  public iOSTargetDevice target_device;
  public string iOSVersion = "9.0";
  public AndroidSdkVersions minSdk = AndroidSdkVersions.AndroidApiLevel21; // android 5.0
#endif

  [Header("misc")]
  public bool developementBuild = false;
  public bool use_joystick_visualisation = true;
  public bool openFolderOnBuildSuccess = false;

#if UNITY_EDITOR
  public UIOrientation orientation_default = UIOrientation.Portrait;
#endif

  [Header("icons")]
  public Texture2D icon_ios;
  public Texture2D icon_android;

  [Header("splashscreen")]
  public Sprite splashscreen;
  
}
