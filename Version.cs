
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

  [ContextMenu("Increase Version")]
  protected void increase_minor()
  {
    minor++;
    fix = 0;
    build++;

    applyVersion();
  }

  [ContextMenu("Increase Fix")]
  protected void increase_fix()
  {
    fix++;
    build++;

    applyVersion();
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

    Debug.Log("<b>DIRTY</b>");
    EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
  }
  
#endif
}
