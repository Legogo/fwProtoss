using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// carry de tout les blueprints de touches d'un objet
/// </summary>

public class InputKeyManager {

  static protected Dictionary<Type, InputKey> list = new Dictionary<Type, InputKey>();
  
  static public InputKey get<T>() where T : InputKey
  {
    return list[typeof(T)];
  }

  static public void create<T>() where T : InputKey
  {
    if (!list.ContainsKey(typeof(T))) list.Add(typeof(T), default(T));
  }

  static public void remove<T>() where T : InputKey
  {
    if (!list.ContainsKey(typeof(T))) return;
    list.Remove(typeof(T));
  }

}
