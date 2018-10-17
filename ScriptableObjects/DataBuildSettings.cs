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
  
  #if UNITY_EDITOR
  public UIOrientation orientation_default = UIOrientation.Portrait;
#endif

  public bool developementBuild = false;
  public bool use_joystick_visualisation = true;
  
}
