using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperMaterial {
  

  static public Material extractSharedMaterial(string matName, Renderer target)
  {

    if (target.sharedMaterial.name.Contains(matName)) return target.sharedMaterial;

    for (int i = 0; i < target.sharedMaterials.Length; i++)
    {
      if (target.sharedMaterials[i].name.Contains(matName)) return target.sharedMaterials[i];
    }

    Debug.LogWarning("can't extract a shared shader of name " + matName + " in " + target.name, target.transform);

    return null;
  }
}
