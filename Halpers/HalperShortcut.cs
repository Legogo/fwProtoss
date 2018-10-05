using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// query helper
/// </summary>
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
