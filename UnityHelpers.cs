using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

public class qh
{
  static public T gc<T>() where T : UnityEngine.Object
  {
    return GameObject.FindObjectOfType<T>();
  }

  static public T dupl<T>(GameObject refObject) where T : UnityEngine.Object
  {
    return GameObject.Instantiate(refObject).GetComponent<T>();
  }
}

public class UnityHelpers {

  /// <summary> Converts given bitmask to layer number </summary>
  /// <returns> layer number </returns>
  public static int ToLayer(int bitmask)
  {
    int result = bitmask > 0 ? 0 : 31;
    while (bitmask > 1)
    {
      bitmask = bitmask >> 1;
      result++;
    }
    return result;
  }

  static public bool isInLayerMask(GameObject obj, LayerMask layerMask)
  {
    return ((layerMask.value & (1 << obj.layer)) > 0);
  }

  static public Collider2D[] getColliders2D(Transform parent)
  {
    List<Collider2D> tmp = new List<Collider2D>();
    tmp.AddRange(parent.GetComponents<Collider2D>());
    tmp.AddRange(parent.GetComponentsInChildren<Collider2D>());
    return tmp.ToArray();
  }


#if UNITY_EDITOR
  [MenuItem("Tools/Clear console #&c")]
  public static void ClearConsole()
  {
    var assembly = Assembly.GetAssembly(typeof(SceneView));
    var type = assembly.GetType("UnityEditor.LogEntries");
    var method = type.GetMethod("Clear");
    method.Invoke(new object(), null);
  }
  [MenuItem("Tools/Clear PlayerPrefs")]
  public static void ClearPlayerPrefs()
  {
    PlayerPrefs.DeleteAll();
    PlayerPrefs.Save();
  }
#endif

}
