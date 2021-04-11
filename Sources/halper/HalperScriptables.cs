using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
[CreateAssetMenu(menuName = "protoss/create DataClass", order = 100)]
public class DataClass : ScriptableObject
{
}
*/

static public class HalperScriptables {

#if UNITY_EDITOR

  static public T[] getScriptableObjectsInEditor<T>() where T : ScriptableObject
  {
    string[] all = AssetDatabase.FindAssets("t:" + typeof(T).Name);
    List<T> output = new List<T>();
    for (int i = 0; i < all.Length; i++)
    {
      Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(T));
      T data = obj as T;
      if (data == null) continue;
      output.Add(data);
    }
    return output.ToArray();
  }

  static public T getScriptableObjectInEditor<T>(string nameContains = "") where T : ScriptableObject
  {
    string[] all = AssetDatabase.FindAssets("t:"+typeof(T).Name);
    for (int i = 0; i < all.Length; i++)
    {
      Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(T));
      T data = obj as T;

      if (data == null) continue;
      if(nameContains.Length > 0)
      {
        if (!data.name.Contains(nameContains)) continue;
      }

      return data;
    }
    Debug.LogWarning("can't locate scriptable of type " + typeof(T).Name + " (filter name ? " + nameContains + ")");
    return null;
  }
#endif

}
