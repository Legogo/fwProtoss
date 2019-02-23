using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// sound need a mixer setup on EngineManager with 3 group with exposed volume params (master, fx, music)
/// 
/// https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
/// https://docs.unity3d.com/ScriptReference/PlayerSettings.SetPlatformIcons.html
/// </summary>

static public class GlobalSettingsSystem {
  
  public const string ppref_fullscreen = "ppref_fullscreen";
  public const string ppref_resolution = "ppref_resolution";
  
  static public void setupTraceLog()
  {
    EngineManager em = EngineManager.get();
    if(em != null)
    {
      Application.SetStackTraceLogType(LogType.Log, em.normal);
      Application.SetStackTraceLogType(LogType.Warning, em.warning);
      Application.SetStackTraceLogType(LogType.Error, em.error);
      
      if(!Application.isEditor)
      {
        //preset
        //do be too wordy on smartphones
        if (em.mobile_logs_preset && Application.isMobilePlatform)
        {
          Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
          Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
          Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
        }
      }
    }

    Debug.Log("trace logs setup");
    Debug.Log("  L log  " + Application.GetStackTraceLogType(LogType.Log));
    Debug.Log("  L warning  " + Application.GetStackTraceLogType(LogType.Warning));
    Debug.Log("  L error  " + Application.GetStackTraceLogType(LogType.Error));

    //GlobalSettingsVolume.setupVolumes();
  }
  
  static public string getSystemInfo() {
    string str = "<color=red>SYSTEM INFO</color>";

#if UNITY_IOS
    str += "\n[iphone generation]"+UnityEngine.iOS.Device.generation.ToString();
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

    str += "\n[support gyro]" + SystemInfo.supportsGyroscope;
    str += "\n[support accelero]" + SystemInfo.supportsAccelerometer;

    str += "\n[processor count]" + SystemInfo.processorCount;
    str += "\n[processor type]" + SystemInfo.processorType;
    str += "\n[support 3d texture]" + SystemInfo.supports3DTextures;
    str += "\n[support shadow]" + SystemInfo.supportsShadows;

    str += "\n[platform] " + Application.platform;
    str += "\n[screen size] " + Screen.width + " x " + Screen.height;
    str += "\n[screen pixel density dpi] " + Screen.dpi;

    return str;
  }

  static public void setupResolution()
  {
    bool fs = PlayerPrefs.GetInt(ppref_fullscreen, 1) == 1;

    string res = PlayerPrefs.GetString(ppref_resolution, "none");

    //https://answers.unity.com/questions/16216/how-to-get-native-screen-resolution.html
    Resolution resolution = Screen.currentResolution;
    if (res.ToLower() == "none") res = resolution.width + "x" + resolution.height; // just whatever is setup right now
    else if (res.ToLower() == "native") res = getHighestResolution(); // highest possible resolution for this screen
    else if (res.ToLower() == "default") res = "1920x1080";

    string[] sizes = res.Split('x');
    Vector2 outputRes = new Vector2(int.Parse(sizes[0]), int.Parse(sizes[1]));

    Debug.Log("~SettingsManager~ setup resolution to " + outputRes + " (ppref ? " + res + ") , fullscreen ? " + fs);
    Screen.SetResolution((int)outputRes.x, (int)outputRes.y, fs);

    PlayerPrefs.Save();
  }

  static public string getHighestResolution()
  {
    //https://docs.unity3d.com/ScriptReference/Screen-resolutions.html
    Resolution[] resolutions = Screen.resolutions;
    int w = 0;
    int h = 0;
    foreach (Resolution res in resolutions)
    {
      w = Mathf.Max(w, res.width);
      h = Mathf.Max(h, res.height);
    }
    return w + "x" + h;
  }


}
