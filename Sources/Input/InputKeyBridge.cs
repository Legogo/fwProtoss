using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// carry de tout les blueprints de touches d'un objet
/// </summary>

namespace fwp.input
{
  public class InputKeyBridge
  {

    protected Dictionary<Type, InputKey> list = new Dictionary<Type, InputKey>();

    public T get<T>() where T : InputKey
    {
      if (list.ContainsKey(typeof(T))) return list[typeof(T)] as T;

      T ik = (T)Activator.CreateInstance(typeof(T));
      list.Add(typeof(T), ik as InputKey);
      //Debug.Log("created : " + ik);

      return ik;
    }

    public void remove<T>() where T : InputKey
    {
      if (!list.ContainsKey(typeof(T))) return;
      list.Remove(typeof(T));
    }

  }
}