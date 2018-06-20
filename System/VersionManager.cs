using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 
///PlayerSettings.Android.v
///PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android)
///
///https://mogutan.wordpress.com/2015/03/06/confusing-unity-mobile-player-settings-for-versions/

///PlayerSettings.bundleVersion = major + "." + minor + "."+increment;
///PlayerSettings.Android.bundleVersionCode = build;
///PlayerSettings.iOS.buildNumber = PlayerSettings.bundleVersion;
/// </summary>

public class VersionManager : MonoBehaviour
{

  [RuntimeInitializeOnLoadMethod]
  static public void logVersion()
  {
    //https://docs.unity3d.com/Manual/StyledText.html
    Debug.Log("<color=teal>v" + getFormatedVersion() + "</color>");
  }

  static public string getFormatedVersion(int[] data = null)
  {
    if (data == null) data = getVersion();
    return data[0] + "." + data[1] + "." + data[2];
  }

  static private int[] getVersion()
  {

    string v = "";
#if UNITY_EDITOR
    //v = PlayerSettings.bundleVersion;
    v = Application.version;
#else
    v = Application.version;
#endif

    string[] split = v.Split('.');
    int[] output = new int[split.Length];
    for (int i = 0; i < split.Length; i++)
    {
      output[i] = int.Parse(split[i]);
    }
    return output;
    //return new int[] { int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]) };
  }



#if UNITY_EDITOR

  [MenuItem("Version/log current")]
  static protected void menuLogVersion()
  {
    logVersion();
  }

  [MenuItem("Version/Increment X.minor.build")]
  private static void incrementMajor()
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
  private static void incrementFix()
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
  
  static private void apply(int[] data)
  {
    PlayerSettings.Android.bundleVersionCode++;

    PlayerSettings.bundleVersion = getFormatedVersion(data);
    PlayerSettings.iOS.buildNumber = PlayerSettings.bundleVersion;

    logVersion();
  }

#endif

  }
