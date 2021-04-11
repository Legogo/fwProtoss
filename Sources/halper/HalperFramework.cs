using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperFramework {

  static public T findManager<T>(string nm) where T : Component
  {
    T t = GameObject.FindObjectOfType<T>();
    if (t == null)
    {
      GameObject obj = new GameObject("(" + nm + ")");
      t = obj.AddComponent<T>();
    }
    return t;
  }

  /// <summary>
  /// will GO.Active = flag all children of transform
  /// </summary>
  static public void toggleAnchor(string nm, bool flag)
  {
    Transform tr = findAnchor(nm);
    if (tr == null) return;

    foreach (Transform child in tr)
    {
      child.gameObject.SetActive(flag);
    }
  }

  static public Transform findAnchor(string nm)
  {
    //remove starting #
    if (nm[0] == '#') nm = nm.Substring(1, nm.Length - 2);

    GameObject obj = GameObject.Find("#" + nm);
    if (obj != null) return obj.transform;

    Debug.LogWarning("can't find " + nm + " in context");

    return null;
  }

  static public T getComponentOfAnchor<T>(string nm) where T : Component
  {
    Transform tr = findAnchor(nm);
    if (tr == null) return null;

    T t = tr.GetComponent<T>();
    if (t == null) t = tr.GetComponentInChildren<T>();
    return t;
  }

}
