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

static public class VersionManager
{

  [RuntimeInitializeOnLoadMethod]
  static public void logVersion()
  {
    //https://docs.unity3d.com/Manual/StyledText.html
    Debug.Log("<color=teal>v" + getFormatedVersion() + "</color>");
  }

  /// <summary>
  /// major.minor.inc
  /// </summary>
  static public string getFormatedVersion(char separator = '.', int[] data = null)
  {
    if (data == null) data = getVersion();
    return ""+ data[0] + separator + data[1] + separator + data[2];
  }
  
  static private int[] getVersion()
  {
    string v = "";

    v = Application.version;

    //Debug.Log(v);

    if(v.Length < 1 || v.IndexOf(".") < 0)
    {
      v = "0.0.1";
    }

    List<string> split = new List<string>();
    split.AddRange(v.Split('.'));
    
    //Debug.Log(split.Count);

    if (split.Count < 1) split.Add("0");
    if (split.Count < 2) split.Add("0");
    if (split.Count < 3) split.Add("0");

    //Debug.Log(split.Count);

    int[] output = new int[split.Count];
    for (int i = 0; i < split.Count; i++)
    {
      output[i] = int.Parse(split[i]);
    }
    return output;
    //return new int[] { int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]) };
  }



#if UNITY_EDITOR

  [MenuItem("Version/log current")]
  static public void menuLogVersion()
  {
    logVersion();
  }

  [MenuItem("Version/Increment X.minor.build")]
  static public void incrementMajor()
  {
    int[] v = getVersion();

    v[0]++;
    if (v.Length > 1) v[1] = 0;
    if (v.Length > 2) v[2] = 0;
    apply(v);
  }

  [MenuItem("Version/Increment major.X.build")]
  private static void incrementMinor()
  {
    int[] v = getVersion();

    if (v.Length < 2)
    {
      List<int> tmp = new List<int>();
      tmp.AddRange(v);
      tmp.Add(0);
      v = tmp.ToArray();
    }

    v[1]++;
    if (v.Length > 2) v[2] = 0;

    apply(v);
  }

  [MenuItem("Version/Increment major.minor.X")]
  public static void incrementFix()
  {
    int[] v = getVersion();

    if (v.Length < 3)
    {
      List<int> tmp = new List<int>();
      tmp.AddRange(v);
      tmp.Add(0);
      v = tmp.ToArray();
    }

    v[2]++;

    apply(v);
  }
  
  static public void incrementBuildNumber()
  {
    PlayerSettings.Android.bundleVersionCode++; // shared with ios ?
    PlayerSettings.iOS.buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
  }

  static private void apply(int[] data, bool incBuildVersion = true)
  {
    if(incBuildVersion) incrementBuildNumber();

    PlayerSettings.bundleVersion = getFormatedVersion('.', data);
    //PlayerSettings.iOS.buildNumber = PlayerSettings.bundleVersion;

    logVersion();
  }

#endif

  }
