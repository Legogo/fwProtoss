using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperGameObject {

  /// <summary>
  /// This will create gameobject if it doest exist
  /// </summary>
  /// <param name="nm"></param>
  /// <returns></returns>
  static public GameObject getGameObject(string nm)
  {
    GameObject obj = GameObject.Find(nm);
    if (obj == null) obj = new GameObject(nm);
    return obj;
  }

}
