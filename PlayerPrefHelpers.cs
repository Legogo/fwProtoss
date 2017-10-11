using System.Collections;
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
    if (addVal.Length <= 0)
    {
      Debug.LogError("addValue should not be empty");
      return;
    }

    //already added
    if (has(addVal))
    {
      Debug.LogWarning("value : "+addVal+" , already added to StringPref");
      return;
    }

    string data = get();
    
    if (data.Length <= 0) data = addVal;
    else data += "," + addVal;

    PlayerPrefs.SetString(key, data);
    PlayerPrefs.Save();

    //Debug.Log("SAVE | " + key + " = " + data);
  }

  public string get()
  {
    return PlayerPrefs.GetString(key, "");
  }

  public int count()
  {
    string ct = get();

    if(ct.Length > 0 && ct.IndexOf(",") > -1)
    {
      return ct.Split(',').Length;
    }

    return 0;
  }

  public bool has(int compVal) { return has(compVal.ToString()); }
  public bool has(string compVal)
  {
    string data = get();

    //Debug.Log(GetType()+"  now searching for '" + compVal + "' in '" + data+"'");

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

    //Debug.Log(GetType() + "    not found " +compVal);

    return false;
  }
}
