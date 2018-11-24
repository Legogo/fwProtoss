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

  static public void scaleRenderToBoxColliderBounds(Transform target, BoxCollider collider, float step)
  {
    step = Mathf.Abs(step); // just in case ...

    Renderer render = HalperComponentsGenerics.getComponent<Renderer>(target);

    float colSize = Vector3.Distance(collider.bounds.min, collider.bounds.max);

    float renderSize = 0f;

    bool doneGrowing = false;
    
    int safe = 100;

    Debug.Log("  >>>> <b>scaling " + target.name+"</b> <<<< ", target);

    while (safe > 0 && !doneGrowing)
    {
      renderSize = Vector3.Distance(render.bounds.min, render.bounds.max);

      float diff = colSize - renderSize;

      Debug.Log("  L step : " + step + " > diff " + diff);

      if (Mathf.Abs(diff) < step)
      {
        doneGrowing = true;
        continue;
      }

      float dir = Mathf.Sign(diff);

      Vector3 scaleStep = Vector3.one * step * dir;

      Debug.Log("  L scale step : " + scaleStep);
      Debug.Log("  L render scale (before) : " + target.localScale);

      target.localScale += scaleStep;

      Debug.Log("  L render scale (after) : " + target.localScale, target);
      Debug.Log("  L render bounds min : " + render.bounds.min);
      Debug.Log("  L render bounds max : " + render.bounds.max);

      //Debug.Log("  L (" + safe + ") " + render.name + " growing to " + render.transform.localScale);
      safe--;
    } 

    if (safe <= 0)
    {
      Debug.LogError("safe!");
    }

    Debug.Log(target.name + " is done growing in " + collider.name, target);
  }

  static public void growScaleInBoxCollider(Transform target, BoxCollider collider, float step)
  {
    Renderer render = HalperComponentsGenerics.getComponent<Renderer>(target);

    int safe = 100;
    while(safe > 0 && collider.bounds.Contains(render.bounds.min) && collider.bounds.Contains(render.bounds.max))
    {
      render.transform.localScale *= (1f + step);
      safe--;
    }

    if(safe <= 0)
    {
      Debug.LogError("safe!");
    }

    Debug.Log(target.name + " is done growing in " + collider.name, target);
  }
}
