using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class SaveTools
{

  static public T getInstance<T>(string instanceId)
  {
    string json = PlayerPrefs.GetString(instanceId, "");
    if (json.Length > 3) return JsonUtility.FromJson<T>(json);
    return default(T);
  }
  
}
