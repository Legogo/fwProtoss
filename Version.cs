
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
#endif

using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Version : MonoBehaviour {

  public int major = 0;
  public int minor = 0;
  public int fix = 0;
  public int build = 1;

#if UNITY_EDITOR

  public Text txt;
  
  [MenuItem("Version/Increase Version")]
  static protected void increase_minor()
  {
    Version v = GameObject.FindObjectOfType<Version>();

    v.minor++;
    v.fix = 0;
    v.build++;

    v.applyVersion();
  }

  [MenuItem("Version/Increase Fix")]
  static protected void increase_fix()
  {
    Version v = GameObject.FindObjectOfType<Version>();

    v.fix++;
    v.build++;

    v.applyVersion();
  }

  protected void applyVersion() {

    if (txt == null) txt = GetComponent<Text>();
    //https://mogutan.wordpress.com/2015/03/06/confusing-unity-mobile-player-settings-for-versions/

    PlayerSettings.bundleVersion = major + "." + minor + "." + fix + "-" + build;
    PlayerSettings.Android.bundleVersionCode = build;
    PlayerSettings.iOS.buildNumber = PlayerSettings.bundleVersion;

    if (txt != null)
    {
      txt.text = "" + PlayerSettings.bundleVersion;
    }

    Debug.Log("<b>DIRTY</b> "+PlayerSettings.bundleVersion);
    EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
  }
  
#endif
  
}
