
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
  public int version = 0;
  public int build = 1;

#if UNITY_EDITOR

  public Text txt;

  private void Awake()
  {
    applyVersion();
  }

  [MenuItem("Version/Increase Minor")]
  static protected void increase_minor()
  {
    Version v = GameObject.FindObjectOfType<Version>();

    v.minor++;
    v.version = 0;

    v.build++;

    v.applyVersion();
  }

  [MenuItem("Version/Increase Version")]
  static protected void increase_fix()
  {
    Version v = GameObject.FindObjectOfType<Version>();

    v.version++;
    v.build++;

    v.applyVersion();
  }

  protected string formatedVersion()
  {
    string str = major + "." + minor + "." + version + "-" + build;
#if sdk
    str += "-sdk";
#endif
    return str;
  }

  protected void applyVersion() {

    if (txt == null) txt = GetComponent<Text>();
    //https://mogutan.wordpress.com/2015/03/06/confusing-unity-mobile-player-settings-for-versions/

#if UNITY_EDITOR
    PlayerSettings.bundleVersion = formatedVersion();
    PlayerSettings.Android.bundleVersionCode = build;
    PlayerSettings.iOS.buildNumber = PlayerSettings.bundleVersion;
    
    Debug.Log("<b>DIRTY</b> " + PlayerSettings.bundleVersion);
    EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

    if (txt != null) txt.text = PlayerSettings.bundleVersion.ToString();

#else
    
    if (txt != null) txt.text = formatedVersion();

#endif


  }



}
