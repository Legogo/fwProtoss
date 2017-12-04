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

    //if (child == null) Debug.LogWarning("couldn't find child of name " + name + " in hierarchy of " + context.name);

    return child;
  }

  static public Transform getCarrierByContext(string name, Transform context = null)
  {
    return extractChildOfName(name, context);
  }

  static public Transform getCarrier(string name)
  {
    GameObject output = GameObject.Find(name);
    
    if (output != null) {
      return output.transform;
    }

    Debug.LogWarning("couldn't find carry of name " + name);
    return null;
  }

  static public T getComponentInSceneByContext<T>(string carryName, Transform context)
  {
    return getCarrierByContext(carryName, context).GetComponent<T>();
  }

  static public T getComponentInScene<T>(string carryName)
  {
    Transform obj = getCarrier(carryName);
    if(obj != null) return obj.GetComponent<T>();
    return default(T);
  }

}

[Serializable]
public class ObjectFetch
{
  public string carry = "";
  protected Component comp;
  
  public T getComponentOnce<T>() where T : Component
  {
    if(comp == null) comp = getComponent<T>(carry);
    return (T)comp;
  }

  public T getComponent<T>(string carry)
  {
    Transform obj = EngineTools.getCarrier(carry);
    if (obj == null)
    {
      Debug.LogWarning("no object found with name : " + carry);
      return default(T);
    }

    return obj.GetComponent<T>();
  }
}
