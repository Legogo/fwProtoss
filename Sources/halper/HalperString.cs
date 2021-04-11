using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

static public class HalperString
{
  static public string iStringFormatBool(string label, bool val)
  {
    return label + " ? " + (val ? "<color=green>true</color>" : "<color=red><b>false</b></color>");
  }

  static public string addZeros(int val, int digit = 2)
  {
    string output = val.ToString();
    if (digit >= 2 && val < 10) output = "0" + output;
    if (digit >= 3 && val < 100) output = "0" + output;
    return output;
  }

  static public string upperFirstLetter(this string v)
  {
    return v.Substring(0, 1).ToUpper() + v.Substring(1, v.Length - 1);
  }

  static public string lowerFirstLetter(this string v)
  {
    return v.Substring(0, 1).ToLower() + v.Substring(1, v.Length - 1);
  }


  static public string[] extractNoneEmptyLines(string[] lines)
  {
    List<string> output = new List<string>();
    for (int i = 0; i < lines.Length; i++)
    {
      if (lines[i].Length > 0) output.Add(lines[i]);
    }
    return output.ToArray();
  }

  /// <summary>
  /// MM:SS:MMM
  /// </summary>
  static public string getFormatedTimeMSM(int seconds, string separator = ":")
  {
    TimeSpan ts = new TimeSpan(0, 0, seconds);
    return ts.Minutes + separator + ts.Seconds + separator + ts.Milliseconds;
  }
}
