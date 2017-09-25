using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EngineTools {

  static public Transform extractChildOfName(string name, Transform context) {
    Transform child = context.Find(name);
    if (child != null) return child;

    foreach(Transform tr in context) {
      child = extractChildOfName(name, tr);
    }

    if (child == null) Debug.LogWarning("couldn't find child of name " + name + " in hierarchy of " + context.name);

    return child;
  }

  static public Transform getCarrier(string name, Transform context = null)
  {
    GameObject output;

    if (context != null) {
      return extractChildOfName(name, context);
    }

    output = GameObject.Find(name);
    
    if (output != null) {
      return output.transform;
    }

    Debug.LogWarning("couldn't find carry of name " + name);
    return null;
  }

  static public T getComponentInScene<T>(string carryName, Transform context) {
    return getCarrier(carryName).GetComponent<T>();
  }

  static public T getComponentInScene<T>(string carryName)
  {
    return getCarrier(carryName).GetComponent<T>();
  }

}

[Serializable]
public class ObjectFetch
{
  public string carry = "";
  protected Component comp;
  
  public T getComponent<T>() where T : Component
  {
    if(comp == null) comp = getComponent<T>(carry);
    return (T)comp;
  }

  public T getComponent<T>(string carry)
  {
    Transform obj = EngineTools.getCarrier(carry);
    if (obj == null)
    {
      Debug.LogError("no object found with name : " + carry);
      return default(T);
    }

    return obj.GetComponent<T>();
  }
}
