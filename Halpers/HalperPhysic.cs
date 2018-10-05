using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalperPhysic {

  static public Collider2D[] getColliders2D(Transform parent)
  {
    List<Collider2D> tmp = new List<Collider2D>();
    tmp.AddRange(parent.GetComponents<Collider2D>());
    tmp.AddRange(parent.GetComponentsInChildren<Collider2D>());
    return tmp.ToArray();
  }

  static public Collider fetchCapsuleColliderInParent(Transform tr)
  {
    CapsuleCollider _tmp = null;

    while (_tmp == null && tr != null)
    {
      _tmp = tr.GetComponent<CapsuleCollider>();
      if (_tmp == null) tr = tr.parent;
    }

    return _tmp;
  }

  static public Collider fetchColliderInParent(Transform tr)
  {
    Collider _tmp = null;

    while (_tmp == null && tr != null)
    {
      _tmp = tr.GetComponent<Collider>();
      if (_tmp == null) tr = tr.parent;
    }

    return _tmp;
  }
}
