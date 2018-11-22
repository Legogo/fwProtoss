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

  /// <summary>
  /// this will destroy gameobject of mono if go only contains that mono and no other
  /// </summary>
  /// <param name="mono"></param>
  static public void checkDestroyOnSoloMono(MonoBehaviour mono)
  {
    MonoBehaviour[] monos = mono.gameObject.GetComponents<MonoBehaviour>();
    if(monos.Length == 1)
    {
      GameObject.DestroyImmediate(mono.gameObject);
    }
    else
    {
      //Debug.LogWarning("gameobject has " + monos.Length + " other monos, won't destroy");
      GameObject.DestroyImmediate(mono);
    }
  }
}
