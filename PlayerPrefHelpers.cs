﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Some upper layer of abstraction to manipulate PlayerPrefs
/// 
/// </summary>

public class StringPref
{
  protected string key = "";

  public StringPref(string key)
  {
    this.key = key;
  }

  public void reset()
  {
    PlayerPrefs.SetString(key, "");
  }

  public void addValue(string addVal)
  {
    //already added
    if (has(addVal)) return;

    string data = get();

    if (data.Length <= 0) data = addVal;
    else data += "," + addVal;

    PlayerPrefs.SetString(key, data);
  }

  public string get()
  {
    return PlayerPrefs.GetString(key, "");
  }

  public bool has(int compVal) { return has(compVal.ToString()); }
  public bool has(string compVal)
  {
    string data = get();

    if (data.Length <= 0) return false;

    if (data.IndexOf(',') > -1)
    {
      string[] split = data.Split(',');
      for (int i = 0; i < split.Length; i++)
      {
        if (split[i] == compVal) return true;
      }
    }
    else
    {
      if (data == compVal) return true;
    }

    return false;
  }
}
