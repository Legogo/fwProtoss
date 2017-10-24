using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "data/new DataSettings", order = 100)]
public class DataSettings : ScriptableObject
{
  public string compagny_name = "playwing";
  public string product_name = "Snake Smash";
  public string package_name = "com.playwing.snakesmash";
  
  #if UNITY_EDITOR
  public UIOrientation orientation_default = UIOrientation.Portrait;
#endif

  //public bool use_ingame_coin_rwd_video = true;
  public bool use_joystick_visualisation = true;
  
}
