using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperComponentsGenerics
{

  static public T getComponent<T>() where T : Component
  {
    return GameObject.FindObjectOfType<T>();
  }

  /// <summary>
  /// on itself or in any children
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="comp"></param>
  /// <returns></returns>
  static public T getComponent<T>(Component comp) where T : Component
  {
    T t = comp.GetComponent<T>();
    if (t == null) t = comp.GetComponentInChildren<T>();
    return t;
  }

  static public T getComponent<T>(GameObject elmt) where T : Component
  {
    T t = elmt.GetComponent<T>();
    if (t == null) t = elmt.GetComponentInChildren<T>();
    return t;
  }

  static public T getComponent<T>(string carryName) where T : Component
  {
    GameObject obj = GameObject.Find(carryName);
    if (obj == null) return null;

    T t = obj.GetComponent<T>();
    if (t == null) t = obj.GetComponentInChildren<T>();
    return null;
  }

  public static T getComponentContext<T>(Transform tr, string endName) where T : Component
  {
    tr = HalperTransform.findChild(tr, endName);
    if (tr == null) return null;
    return tr.GetComponent<T>();
  }

  public static T getComponentByCarryName<T>(string carryName) where T : Component
  {
    GameObject obj = GameObject.Find(carryName);

    if (obj == null)
    {
      //Debug.LogWarning("couldn't find " + carryName);
      return null;
    }

    return getComponent<T>(obj);
  }


  static public T getManager<T>(string nm) where T : MonoBehaviour
  {
    GameObject obj = GameObject.Find(nm);
    T tmp = null;
    if (obj != null)
    {
      tmp = obj.GetComponent<T>();
    }

    if (tmp != null) return tmp;

    if (obj == null)
    {
      obj = new GameObject(nm, typeof(T));
      tmp = obj.GetComponent<T>();
    }
    else tmp = obj.AddComponent<T>();

    return tmp;
  }




  static public T getManager<T>(string nm, bool dontDestroy = false) where T : MonoBehaviour
  {
    GameObject obj = GameObject.Find(nm);
    T tmp = null;
    if (obj != null)
    {
      tmp = obj.GetComponent<T>();
    }

    if (tmp != null) return tmp;

    if (obj == null)
    {
      obj = new GameObject(nm, typeof(T));
      tmp = obj.GetComponent<T>();
    }
    else tmp = obj.AddComponent<T>();

    if (dontDestroy) GameObject.DontDestroyOnLoad(tmp);

    return tmp;
  }

}
