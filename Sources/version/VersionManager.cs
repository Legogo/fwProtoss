using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 
/// PlayerSettings.Android.v
/// PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android)
///
/// https://mogutan.wordpress.com/2015/03/06/confusing-unity-mobile-player-settings-for-versions/
/// 
/// PlayerSettings.bundleVersion = major + "." + minor + "."+increment;
/// PlayerSettings.Android.bundleVersionCode = build;
/// PlayerSettings.iOS.buildNumber = PlayerSettings.bundleVersion;
///
/// https://stackoverflow.com/questions/21125159/which-ios-app-version-build-numbers-must-be-incremented-upon-app-store-release
/// "Version" CFBundleShortVersionString(String - iOS, OS X) specifies the release version number of the bundle, which identifies a released iteration of the app.The release version number is a string comprised of three period-separated integers.
/// "Build" CFBundleVersion (String - iOS, OS X) specifies the build version number of the bundle, which identifies an iteration(released or unreleased) of the bundle.The build version number should be a string comprised of three non-negative, period-separated integers with the first integer being greater than zero.The string should only contain numeric (0-9) and period(.) characters.Leading zeros are truncated from each integer and will be ignored(that is, 1.02.3 is equivalent to 1.2.3). This key is not localizable.``
/// 
/// guide lines
/// https://stackoverflow.com/questions/21125159/which-ios-app-version-build-numbers-must-be-incremented-upon-app-store-release/38009895#38009895
/// 
/// The pair(Version, Build number) must be unique.
///   The sequence is valid: (1.0.1, 12) -> (1.0.1, 13) -> (1.0.2, 13) -> (1.0.2, 14) ...
///   Version(CFBundleShortVersionString) must be in ascending sequential order.
///   Build number(CFBundleVersion) must be in ascending sequential order.
///   
/// </summary>

public class VersionManager : MonoBehaviour
{

  public const char versionSeparator = '.';

  float timer = 4f;
  
  private void Update()
  {
    timer -= Time.deltaTime;
    if(timer <= 0f)
    {
      GameObject.Destroy(gameObject);
    }
  }

  private void OnGUI()
  {
    string v = Application.version;

    GUIStyle guis = new GUIStyle();

    guis.normal.textColor = Color.gray;

    guis.fontSize = 30;
    float width = 150f;
    float height = 50f;

    if (Screen.width >= 1000f)
    {
      guis.fontSize = 50;
      width = 300f;
      height = 150f;
    }

    // must be safe from rounded angle on mobile
    
    GUI.Label(new Rect(Screen.width - width, Screen.height - height, width, height), v, guis);
  }
  
  [RuntimeInitializeOnLoadMethod]
  static public void displayOnStartup()
  {
    logVersion();

#if !noversion
    new GameObject("!version").AddComponent<VersionManager>();
#endif
  }

  static public void logVersion()
  {
    //https://docs.unity3d.com/Manual/StyledText.html
    Debug.Log(getFormatedVersion(getApplicationVersion()));
  }

  /// <summary>
  /// major.minor.inc
  /// no build number
  /// </summary>
  static public string getFormatedVersion(int[] data = null)
  {
    if (data == null) data = getApplicationVersion();
    return "" + data[0] + versionSeparator + data[1] + versionSeparator + data[2];
  }
  
  static public string getFormatedVersion(char separator, int[] data = null)
  {
    if (data == null) data = getApplicationVersion();
    return "" + data[0] + separator + data[1] + separator + data[2];
  }

  static private int[] getApplicationVersion()
  {
    string v = "";

    v = Application.version;

    //Debug.Log(v);

    //default
    if(v.Length < 1 || v.IndexOf(".") < 0)
    {
      v = "0.0.1";
    }

    //gather numbers
    List<string> split = new List<string>();
    split.AddRange(v.Split('.'));
    
    //add missing members
    if (split.Count < 1) split.Add("0");
    if (split.Count < 2) split.Add("0");
    if (split.Count < 3) split.Add("0");
    
    //convert to int[]
    int[] output = new int[split.Count];
    for (int i = 0; i < split.Count; i++)
    {
      output[i] = int.Parse(split[i]);
    }
    return output;
  }


#if UNITY_EDITOR

  static public void incrementBuildNumber()
  {
    PlayerSettings.Android.bundleVersionCode++; // shared with ios ?
    PlayerSettings.iOS.buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
  }

  static private void apply(int[] data, bool incBuildVersion = true)
  {
    if (incBuildVersion) incrementBuildNumber();

    PlayerSettings.bundleVersion = getFormatedVersion(data);
    //PlayerSettings.iOS.buildNumber = PlayerSettings.bundleVersion;

    logEditorVersion();
  }

  [MenuItem("Version/log current")]
  static public void menuLogVersion()
  {
    logEditorVersion();
  }

  static public void logEditorVersion()
  {
    Debug.Log("<color=green>v" + getFormatedVersion(getApplicationVersion()) + "</color> - " + getApplicationBuildNumber());
  }

  static public void setApplicationVersion(string v)
  {
    PlayerSettings.bundleVersion = v;
  }

  static public void setApplicationBuildNumber(int num)
  {
    PlayerSettings.Android.bundleVersionCode = num;
  }
  static public int getApplicationBuildNumber()
  {
    return PlayerSettings.Android.bundleVersionCode;
  }

  static public DataBuildSettingVersion getVersion() => HalperScriptables.getScriptableObjectInEditor<DataBuildSettingVersion>("version");

  [MenuItem("Version/inc major")]
  static public void incMajor() => getVersion().incrementMajor();
  [MenuItem("Version/inc minor")]
  static public void incMinor() => getVersion().incrementMinor();
  [MenuItem("Version/inc fix")]
  static public void incFix() => getVersion().incrementFix();

#endif

}
