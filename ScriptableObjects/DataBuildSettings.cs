using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "protoss/create DataBuildSettings", order = 100)]
public class DataBuildSettings : ScriptableObject
{
  public string compagny_name = "*";
  public string product_name = "*";
  public string package_name = "com.*.*";

  public bool developementBuild = false;
  public bool use_joystick_visualisation = true;

  public Texture2D icon_ios;
  public Texture2D icon_android;

#if UNITY_EDITOR
  public UIOrientation orientation_default = UIOrientation.Portrait;

  public iOSTargetDevice target_device;
  public string iOSVersion = "9.0";
  
  public AndroidSdkVersions minSdk = AndroidSdkVersions.AndroidApiLevel21; // android 5.0
#endif
}
