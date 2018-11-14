using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// sound need a mixer setup on EngineManager with 3 group with exposed volume params (master, fx, music)
/// </summary>

public class SettingsManager : EngineObject {

  public DataBuildSettings data_settings;

  public const string ppref_sound_master_volume = "ppref_sound_volume_master";
  public const string ppref_sound_music_volume = "ppref_sound_volume_music";
  public const string ppref_sound_fx_volume = "ppref_sound_volume_fx";
  public const string ppref_sound_muted = "ppref_sound_muted";

  public const string ppref_fullscreen = "ppref_fullscreen";
  public const string ppref_resolution = "ppref_resolution";
  
  static public SettingsManager get() {
    return GameObject.FindObjectOfType<SettingsManager>();
  }

  [RuntimeInitializeOnLoadMethod]
  static protected void setupStartupSettings()
  {
    setupResolution();
  }

  protected override void build()
  {
    base.build();
    
    EngineManager em = EngineManager.get();
    if(em != null)
    {
      //preset
      if (em.mobile_logs_preset)
      {
        //do be too wordy on smartphones
        if (Application.isMobilePlatform)
        {
          Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
          Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
          Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
        }
      }
      else
      {
        Application.SetStackTraceLogType(LogType.Log, em.normal);
        Application.SetStackTraceLogType(LogType.Warning, em.warning);
        Application.SetStackTraceLogType(LogType.Error, em.error);
      }

      if (em.log_device_info) Debug.Log(getSystemInfo());
    }

#if UNITY_ANDROID
    setupSonyAndroid();
#endif

    setupVolumes();
  }
  
  protected void setupSonyAndroid()
  {
    
    //sony xperia as a software home menu we have to force "not fullscreen" to keep buttons visible
    //some ad plugin doesn't manage alignment of bottom banner to home menu so it makes it float not anchored to the bottom

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


#if UNITY_EDITOR
  [ContextMenu("apply build settings")]
  public void apply() {
    applySettings(data_settings);
  }

  static public void applySettings(DataBuildSettings data)
  {
    Debug.Log("applying DataBuildSettings ...");

    PlayerSettings.productName = data.product_name;
    PlayerSettings.companyName = data.compagny_name;
    
    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, data.package_name);
    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, data.package_name);
    
    PlayerSettings.defaultInterfaceOrientation = data.orientation_default;
    
    EditorUserBuildSettings.development = data.developementBuild;
  }
#endif

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

  static public void setupVolumes()
  {
    applyMasterVolume();
    applyMusicVolume();
    applyFxVolume();
    //Debug.Log("~SettingsManager~ setup volumes");

    PlayerPrefs.Save();
  }
  
  static protected void applyMasterVolume() { applyVolume("global", ppref_sound_master_volume, PlayerPrefs.GetInt(ppref_sound_muted, 0) == 0 ? 0f : -80f); }
  static protected void applyFxVolume() { applyVolume("fx", ppref_sound_fx_volume); }
  static protected void applyMusicVolume() { applyVolume("music", ppref_sound_music_volume); }

  static protected AnimationCurve volumeCurve = AnimationCurve.EaseInOut(0f,0f,1f,1f);
  static protected float transformVolumeSliderValue(float input)
  {
    //Debug.Log("input "+input+" => "+volumeCurve.Evaluate(input));
    return Mathf.Lerp(-80f, 0f, volumeCurve.Evaluate(input));
  }

  static protected void applyVolume(string category, string ppref_const, float boost = 0f)
  {
    float volume = PlayerPrefs.GetFloat(ppref_const, 1f);
    EngineManager em = GameObject.FindObjectOfType<EngineManager>();
    if (em == null) return;
    if (em.mixer == null) {
      Debug.LogWarning("no mixer ?");
      return;
    }
    volume = transformVolumeSliderValue(volume);
    em.mixer.SetFloat(category, volume + boost);
  }
  
}
