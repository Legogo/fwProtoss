using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SettingsManager : EngineObject {

  public DataSettings data_settings;

  static public SettingsManager get() {
    return GameObject.FindObjectOfType<SettingsManager>();
  }

  protected override void build()
  {
    base.build();

    Application.runInBackground = false;
    
#if UNITY_ANDROID
    Screen.fullScreen = false;
#endif
    
    Debug.Log(getSystemInfo());
  }

  public string getSystemInfo() {
    string str = "<color=red>SYSTEM INFO</color>";

#if UNITY_IOS
    str += "\n[iphone generation]iPhone.generation.ToString()";
#endif

#if UNITY_ANDROID
    str += "\n[system info]" + SystemInfo.deviceModel;
#endif

    str += "\n[type]" + SystemInfo.deviceType;
    str += "\n[os version]" + SystemInfo.operatingSystem;
    str += "\n[system memory size]" + SystemInfo.systemMemorySize;
    str += "\n[graphic device name]" + SystemInfo.graphicsDeviceName + " (version " + SystemInfo.graphicsDeviceVersion + ")";
    str += "\n[graphic memory size]" + SystemInfo.graphicsMemorySize;
    //str += "\n[graphic pixel fill rate]" + SystemInfo.graphicsPixelFillrate;
    str += "\n[graphic max texSize]" + SystemInfo.maxTextureSize;
    str += "\n[graphic shader level]" + SystemInfo.graphicsShaderLevel;
    str += "\n[support compute shader]" + SystemInfo.supportsComputeShaders;

    str += "\n[processor count]" + SystemInfo.processorCount;
    str += "\n[processor type]" + SystemInfo.processorType;
    str += "\n[support 3d texture]" + SystemInfo.supports3DTextures;
    str += "\n[support shadow]" + SystemInfo.supportsShadows;

    str += "\n[platform] " + Application.platform;
    str += "\n[screen size] " + Screen.width + " x " + Screen.height;
    str += "\n[screen pixel density dpi] " + Screen.dpi;

    return str;
  }

  public override void onEngineSceneLoaded()
  {
    base.onEngineSceneLoaded();
    
    TouchController tc = GameObject.FindObjectOfType<TouchController>();
    if(tc != null) {
      tc.useJoyVisualisation = data_settings.use_joystick_visualisation;
    }
  }

#if UNITY_EDITOR
  [ContextMenu("apply")]
  public void apply() {
    PlayerSettings.productName = data_settings.product_name;
    PlayerSettings.companyName = data_settings.compagny_name;
    
    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, data_settings.package_name);
    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, data_settings.package_name);
    
    PlayerSettings.defaultInterfaceOrientation = data_settings.orientation_default;
  }
#endif
}
