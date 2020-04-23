using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

static public class HalperScriptables
{

#if UNITY_EDITOR
  static public T getScriptableObjectInEditor<T>(string nameEnd = "") where T : ScriptableObject
  {
    string nm = typeof(T).Name;
    //Debug.Log(nm);

    string[] all = AssetDatabase.FindAssets("t:"+nm);

    if(all.Length <= 0)
    {
      Debug.LogWarning("no scriptable found for type " + nm);
      return null;
    }

    for (int i = 0; i < all.Length; i++)
    {
      Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(T));
      T data = obj as T;

      //Debug.Log(data);

      if (data == null) continue;
      if(nameEnd.Length > 0)
      {
        if (!data.name.EndsWith(nameEnd)) continue;
      }

      return data;
    }

    Debug.LogWarning("can't locate scriptable of type " + typeof(T).Name + " (filter name ? " + nameEnd + ")");
    return null;
  }

#endif
}
