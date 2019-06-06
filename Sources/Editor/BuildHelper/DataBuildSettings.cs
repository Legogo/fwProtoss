using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

[CreateAssetMenu(menuName = "protoss/create DataBuildSettings", order = 100)]
public class DataBuildSettings : ScriptableObject
{
  public DataBuildSettingProfile activeProfile;

}
