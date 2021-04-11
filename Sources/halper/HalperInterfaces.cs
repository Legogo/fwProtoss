using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperInterfaces
{

  /// <summary>
  /// TROP PA OPTI
  /// </summary>
  static public T[] getCandidates<T>()
  {
    GameObject[] all = GameObject.FindObjectsOfType<GameObject>();
    List<T> tmp = new List<T>();
    for (int i = 0; i < all.Length; i++)
    {
      T inst = all[i].GetComponent<T>();
      if (inst != null)
      {
        tmp.Add(inst);
      }
    }
    return tmp.ToArray();
  }


}
