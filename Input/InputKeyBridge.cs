using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// carry de tout les blueprints de touches d'un objet
/// </summary>

public class InputKeyBridge {

  protected Dictionary<Type, InputKey> list = new Dictionary<Type, InputKey>();

  public InputKey create<T>() where T : new()
  {
    if (list.ContainsKey(typeof(T))) return list[typeof(T)];
    
    T ik = (T)Activator.CreateInstance(typeof(T));
    list.Add(typeof(T), ik as InputKey);
    //Debug.Log("created : " + ik);

    return ik as InputKey;
  }

  public T get<T>() where T : InputKey
  {
    if (!list.ContainsKey(typeof(T))) Debug.LogWarning("can't find InputKey of type " + typeof(T));

    //foreach (KeyValuePair<Type, InputKey> entry in list) Debug.Log(entry.Key+" , "+entry.Value);
    
    InputKey ik = list[typeof(T)];
    return list[typeof(T)] as T;
  }

  public void remove<T>() where T : InputKey
  {
    if (!list.ContainsKey(typeof(T))) return;
    list.Remove(typeof(T));
  }
  
}
