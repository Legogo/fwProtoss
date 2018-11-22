using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

static public class GlobalSettingsBuild {

#if UNITY_EDITOR

  static public DataBuildSettings getScriptableDataBuildSettings()
  {
    string[] all = AssetDatabase.FindAssets("t:DataBuildSettings");
    for (int i = 0; i < all.Length; i++)
    {
      Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(DataBuildSettings));
      DataBuildSettings data = obj as DataBuildSettings;
      if (data != null) return data;
    }
    return null;
  }


  [MenuItem("Build/Apply settings")]
  static public void apply()
  {
    DataBuildSettings data = getScriptableDataBuildSettings();
    applySettings(data);
  }

  static public void applySettings(DataBuildSettings data)
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

    PlayerSettings.SplashScreen.show = false;
    PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, icons);
    EditorUserBuildSettings.development = data.developementBuild;

    //android specific
    PlayerSettings.Android.minSdkVersion = data.minSdk;

    //ios specific
    PlayerSettings.iOS.targetDevice = data.target_device;
    PlayerSettings.iOS.targetOSVersionString = data.iOSVersion;
  }
#endif

}
