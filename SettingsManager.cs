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
    //Screen.fullScreen = false;
#endif
    
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
