using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SettingsManager : EngineObject {

  public DataSettings data_settings;

  public const string ppref_mute_name = "mute";

  static public SettingsManager get() {
    return GameObject.FindObjectOfType<SettingsManager>();
  }

  protected override void build()
  {
    base.build();
    
    //Application.runInBackground = false;

    if (!Application.isEditor)
    {
      Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
      Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
      Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
    }
    
    Debug.Log(getSystemInfo());

#if UNITY_ANDROID

    string[] models = { "sony" };

    Debug.Log("models : " + SystemInfo.deviceModel + " checking for fullscreen removal");

    for (int i = 0; i < models.Length; i++)
    {
      //skip
      if (!Screen.fullScreen) continue;

      Debug.Log("models : " + SystemInfo.deviceModel + " VS "+models[i]);

      if (SystemInfo.deviceModel.ToLower().Contains(models[i].ToLower()))
      {
        Debug.Log("models : " + SystemInfo.deviceModel + " is set for NOT fullscreen");
        Screen.fullScreen = false;
      }
    }

#endif
    
  }
  
  public string getSystemInfo() {
    string str = "<color=red>SYSTEM INFO</color>";

#if UNITY_IOS
    str += "\n[iphone generation]"+iPhone.generation.ToString();
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
    
    /*
    TouchController tc = GameObject.FindObjectOfType<TouchController>();
    if(tc != null) {
      tc.useJoyVisualisation = data_settings.use_joystick_visualisation;
    }
    */
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

  static public void setMuted(bool muted)
  {
    PlayerPrefs.SetFloat(ppref_mute_name, (muted) ? 1f : 0f);
    PlayerPrefs.Save();
  }

  static public bool isMuted()
  {
    return PlayerPrefs.GetFloat(ppref_mute_name, 0f) == 1f;
  }
}
