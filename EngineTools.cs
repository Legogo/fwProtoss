using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EngineTools {

  static public GameObject getCarrier(string name)
  {
    return GameObject.Find(name);
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
    GameObject obj = EngineTools.getCarrier(carry);
    if (obj == null)
    {
      Debug.LogError("no object found with name : " + carry);
      return default(T);
    }

    return obj.GetComponent<T>();
  }
}
