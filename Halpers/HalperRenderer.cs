using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalperRenderer : MonoBehaviour {

  static public MeshRenderer fetchMeshRendererInParent(Transform tr)
  {
    MeshRenderer _tmp = null;
    while (_tmp == null && tr != null)
    {
      _tmp = tr.GetComponent<MeshRenderer>();
      if (_tmp == null) tr = tr.parent;
    }
    return _tmp;
  }

  static public SpriteRenderer fetchSpriteRendererInParent(Transform tr)
  {
    SpriteRenderer _tmp = null;
    while (_tmp == null && tr != null)
    {
      _tmp = tr.GetComponent<SpriteRenderer>();
      if (_tmp == null) tr = tr.parent;
    }
    return _tmp;
  }

}
