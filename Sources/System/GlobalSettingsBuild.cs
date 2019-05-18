using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

static public class GlobalSettingsBuild {

#if UNITY_EDITOR

  static public ScriptableBuildSettingsData getScriptableDataBuildSettings()
  {
    string[] all = AssetDatabase.FindAssets("t:DataBuildSettings");
    for (int i = 0; i < all.Length; i++)
    {
      Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(ScriptableBuildSettingsData));
      ScriptableBuildSettingsData data = obj as ScriptableBuildSettingsData;
      if (data != null) return data;
    }
    return null;
  }


  [MenuItem("Build/Apply settings")]
  static public void apply()
  {
    ScriptableBuildSettingsData data = getScriptableDataBuildSettings();
    applySettings(data);
  }

  static public void applySettings(ScriptableBuildSettingsData data)
  {
    Debug.Log("applying DataBuildSettings ...");

    PlayerSettings.productName = data.product_name;
    PlayerSettings.companyName = data.compagny_name;

    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, data.package_name);
    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, data.package_name);

    PlayerSettings.defaultInterfaceOrientation = data.orientation_default;

    Texture2D[] icons = new Texture2D[1];

#if UNITY_IPHONE
    icons[0] = data.icon_ios;
#elif UNITY_ANDROID
    icons[0] = data.icon_android;
#endif

    PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, icons);

    Debug.Log("  L updated icons");

    PlayerSettings.SplashScreen.show = false;
    //PlayerSettings.SplashScreen.background = data.splashscreen;
    //PlayerSettings.SplashScreenLogo.unityLogo = data.splashscreen;
    //PlayerSettings.SplashScreenLogo.unityLogo = data.splashscreen;
    Debug.Log("  L updated splash");
    
    EditorUserBuildSettings.development = data.developementBuild;

    //android specific
    PlayerSettings.Android.minSdkVersion = data.minSdk;
    Debug.Log("  L updated android stuff");

    //ios specific
    PlayerSettings.iOS.targetDevice = data.target_device;
    PlayerSettings.iOS.targetOSVersionString = data.iOSVersion;
    Debug.Log("  L updated ios stuff");
  }
#endif

}
