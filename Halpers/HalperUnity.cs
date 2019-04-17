using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

static public class HalperUnity {
  
#if UNITY_EDITOR
  static public ScriptableObject getScriptable<T>() where T : ScriptableObject
  {
    string typ = typeof(T).ToString();
    //Debug.Log(typ);
    string[] all = AssetDatabase.FindAssets("t:" + typ);
    //Debug.Log(all.Length);
    for (int i = 0; i < all.Length; i++)
    {
      Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(T));
      T data = obj as T;
      if (data != null) return data;
    }
    return null;
  }
#endif 

  static public List<Object> shuffle<Object>(this List<Object> list)
  {
    for (int i = 0; i < list.Count; i++)
    {
      Object temp = list[i];
      int randomIndex = Random.Range(i, list.Count);
      list[i] = list[randomIndex];
      list[randomIndex] = temp;
    }
    return list;
  }

  static public List<T> loadResources<T>(string path = "") where T : Object
  {
    Object[] list = Resources.LoadAll(path, typeof(T));
    List<T> elements = new List<T>();

    for (int i = 0; i < list.Length; i++)
    {
      if (list[i] as T)
      {
        elements.Add(list[i] as T);
      }
    }

    Debug.Log("loaded " + elements.Count + " objects of type : "+typeof(T).ToString());

    return elements;
  }

  static public Dictionary<string, string[]> loadResourcesLines(string path, string prefix = "")
  {
    List<TextAsset> tmp = loadResources<TextAsset>(path);

    Dictionary<string, string[]> list = new Dictionary<string, string[]>();

    for (int i = 0; i < tmp.Count; i++)
    {
      TextAsset ta = tmp[i];
      bool toAdd = true;
      if (prefix.Length > 0 && !ta.name.ToLower().StartsWith(prefix)) toAdd = false;
      if (toAdd)
      {
        string[] splitted = ta.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        list.Add(ta.name, HalperString.extractNoneEmptyLines(splitted));
      }
    }

    return list;
  }
  
  static public KeyValuePair<string, string[]> loadResourceLine(string path, string prefix)
  {
    Dictionary<string, string[]> files = loadResourcesLines(path, prefix);
    foreach(KeyValuePair<string, string[]> kp in files)
    {
      return kp;
    }
    return default(KeyValuePair<string, string[]>);
  }

  static public void clearGC()
  {
    Debug.Log("clearing GC at frame : " + Time.frameCount);
    Resources.UnloadUnusedAssets();
    System.GC.Collect();
  }

}
