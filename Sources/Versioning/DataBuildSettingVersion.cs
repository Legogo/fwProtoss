using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

[CreateAssetMenu(menuName = "protoss/create DataBuildSettingVersion", order = 100)]
public class DataBuildSettingVersion : ScriptableObject
{
  [Header("version")]
  public string version = "0.0.1";
  public int buildNumber = 1;

#if UNITY_EDITOR
  /// <summary>
  /// override and apply
  /// </summary>
  /// <param name="data"></param>
  /// <param name="buildNum"></param>
  public void apply(int[] data, int buildNum)
  {
    //----- version
    
    version = VersionManager.getFormatedVersion(data);
    VersionManager.setApplicationVersion(version);
    
    //----- build number

    buildNumber = buildNum;
    VersionManager.setApplicationBuildNumber(buildNum);

    Debug.Log("applied : <b>" + version+" - "+buildNumber+"</b>");

    EditorUtility.SetDirty(this);
  }

  public void apply(int major, int minor, int fix, int buildNum)
  {
    List<int> list = new List<int>();
    list.Add(major);
    list.Add(minor);
    list.Add(fix);

    apply(list.ToArray(), buildNum);
  }

  /// <summary>
  /// just apply
  /// </summary>
  public void applyCurrent()
  {
    apply(getDataVersionInts(), buildNumber);
  }

  public void incrementMajor()
  {
    int[] v = getDataVersionInts();

    v[0]++;
    if (v.Length > 1) v[1] = 0;
    if (v.Length > 2) v[2] = 0;

    buildNumber++;

    apply(v, buildNumber);

  }

  public void incrementMinor()
  {
    int[] v = getDataVersionInts();

    if (v.Length < 2)
    {
      List<int> tmp = new List<int>();
      tmp.AddRange(v);
      tmp.Add(0);
      v = tmp.ToArray();
    }

    v[1]++;
    if (v.Length > 2) v[2] = 0;

    buildNumber++;

    apply(v, buildNumber);
  }

  public string incrementFix()
  {
    int[] v = getDataVersionInts();

    if (v.Length < 3)
    {
      List<int> tmp = new List<int>();
      tmp.AddRange(v);
      tmp.Add(0);
      v = tmp.ToArray();
    }

    v[2]++;

    buildNumber++;

    apply(v, buildNumber);

    return getDataVersion();
  }

#endif

  /// <summary>
  /// x.y.z
  /// </summary>
  /// <returns></returns>
  public string getDataVersion()
  {
    //return VersionManager.getFormatedVersion(version);
    return version;
  }

  /// <summary>
  /// int[] [x],[y],[z]
  /// </summary>
  /// <returns></returns>
  public int[] getDataVersionInts()
  {
    List<string> list = new List<string>();
    list.AddRange(version.Split(VersionManager.versionSeparator));

    List<int> output = new List<int>();
    for (int i = 0; i < list.Count; i++)
    {
      output.Add(int.Parse(list[i]));
    }
    return output.ToArray();
  }
  
}
