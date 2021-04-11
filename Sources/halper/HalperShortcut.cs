using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// query helper
/// </summary>
public class qh
{

  static public T[] gcs<T>() where T : UnityEngine.Object
  {
    return GameObject.FindObjectsOfType<T>();
  }

  /// <summary>
  /// gc == getcomponent
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  static public T gc<T>() where T : UnityEngine.Object
  {
    return GameObject.FindObjectOfType<T>();
  }

  static public T gc<T>(string containtName) where T : UnityEngine.Object
  {
    if (containtName.Length <= 0) return gc<T>();

    T[] list = GameObject.FindObjectsOfType<T>();
    for (int i = 0; i < list.Length; i++)
    {
      if (list[i].name.Contains(containtName)) return list[i];
    }
    return null;
  }

  static public T dupl<T>(GameObject refObject) where T : UnityEngine.Object
  {
    return GameObject.Instantiate(refObject).GetComponent<T>();
  }

  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="gameObjectName"></param>
  /// <param name="uniq">there can be only one instance of T, nomater the given name</param>
  /// <returns></returns>
  static public T cr<T>(string gameObjectName, bool uniq = false) where T : UnityEngine.MonoBehaviour
  {
    T tmp = gc<T>();

    if (tmp != null && uniq) return tmp;
    if (tmp != null && tmp.gameObject.name == gameObjectName) return tmp;
    
    GameObject obj = GameObject.Find(gameObjectName);
    if (obj == null) obj = new GameObject(gameObjectName);
    else tmp = obj.GetComponent<T>();

    if (tmp == null) tmp = obj.AddComponent<T>();

    return tmp;
  }
}
