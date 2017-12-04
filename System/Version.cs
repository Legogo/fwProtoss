
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;
using UnityEngine.UI;

public class Version : MonoBehaviour {

  public int major = 0;
  public int minor = 0;
  public int version = 0;
  public int build = 1;
  
  public Text txt;

  private void Awake()
  {
    applyVersionToText();

    #if no_version
    txt.enabled = false;
    #endif
  }

  protected void applyVersionToText()
  {

    if (txt == null) txt = GetComponent<Text>();
    if (txt == null) txt = GetComponentInChildren<Text>();
    if (txt != null) txt.text = formatedVersion();

  }

#if UNITY_EDITOR
  [MenuItem("Version/Increase Minor")]
  static protected void increase_minor()
  {
    Version v = GameObject.FindObjectOfType<Version>();

    v.minor++;
    v.version = 0;

    v.build++;

    v.applyVersionToText();
    v.updatePlayerSettings();
  }

  [MenuItem("Version/Increase Version")]
  static protected void increase_fix()
  {
    Version v = GameObject.FindObjectOfType<Version>();

    v.updatePlayerSettings(true);

    v.version++;
    v.build++;

    v.applyVersionToText();
    v.updatePlayerSettings();
  }


  [MenuItem("Version/Apply current to settings")]
  static protected void apply_current()
  {
    get().updatePlayerSettings();
  }
#endif

  public string formatedVersion()
  {
    string str = major + "." + minor + "." + version + "-" + build;
#if sdk
    str += "-sdk";
#endif
    return str;
  }

  public void updatePlayerSettings(bool mute = false) {

#if UNITY_EDITOR

    //https://mogutan.wordpress.com/2015/03/06/confusing-unity-mobile-player-settings-for-versions/

    if (!Application.isPlaying)
    {
      PlayerSettings.bundleVersion = formatedVersion();
      PlayerSettings.Android.bundleVersionCode = build;
      PlayerSettings.iOS.buildNumber = PlayerSettings.bundleVersion;

      if(!mute) Debug.Log("<b>DIRTY</b> " + PlayerSettings.bundleVersion);
      EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
#endif

  }

  static public Version get() { return GameObject.FindObjectOfType<Version>(); }



}
