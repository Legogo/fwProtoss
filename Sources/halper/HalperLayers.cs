using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalperLayers {

  /// <summary> Converts given bitmask to layer number </summary>
  /// <returns> layer number </returns>
  public static int ToLayer(int bitmask)
  {
    int result = bitmask > 0 ? 0 : 31;
    while (bitmask > 1)
    {
      bitmask = bitmask >> 1;
      result++;
    }
    return result;
  }

  //http://answers.unity3d.com/questions/150690/using-a-bitwise-operator-with-layermask.html
  static public bool isInLayerMask(GameObject obj, LayerMask layerMask)
  {
    return ((layerMask.value & (1 << obj.layer)) > 0);
  }
  static public bool isInLayerMask(GameObject obj, int layerMask)
  {
    return obj.layer == layerMask;
  }

  static public bool hasLayerMask(GameObject obj, string layerName)
  {
    //Debug.Log(obj.layer);
    //Debug.Log(LayerMask.NameToLayer(layerName));
    return obj.layer == LayerMask.NameToLayer(layerName);
  }

  static public void removeLayerMask(GameObject obj) { if (obj.layer != 0) obj.layer = 0; }

  static public void assignLayerMask(GameObject obj, string layerName)
  {
    int newLayer = LayerMask.NameToLayer(layerName);
    if (newLayer != obj.layer) obj.layer = newLayer;
  }
}
