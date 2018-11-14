using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

static public class HalperSystemContextMenu
{
#if UNITY_EDITOR
  [MenuItem("Tools/Clear PlayerPrefs")]
  public static void ClearPlayerPrefs()
  {
    PlayerPrefs.DeleteAll();
    PlayerPrefs.Save();
  }
#endif
}
