using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalperPhysic {

  static public float getRaycastDistance(Vector3 from, Vector3 direction)
  {
    RaycastHit _hit;
    if(Physics.Raycast(from, direction, out _hit, Mathf.Infinity))
    {
      return _hit.distance;
    }
    return Mathf.Infinity;
  }

  static public Vector2 getRaycastPoint(Vector3 from, Vector3 direction, float distance, int layer)
  {
    return getRaycastRay(from, direction, distance, layer).point;
  }

  static public RaycastHit getRaycastRay(Vector3 from, Vector3 direction, float distance, int layer, bool drawDebug = false)
  {
    RaycastHit _hit;

    bool rayed = Physics.Raycast(from, direction, out _hit, distance, 1 << layer);

    if(!drawDebug)
    {
      if (rayed) Debug.DrawLine(from, _hit.point, Color.yellow, 2f);
      else Debug.DrawLine(from, from + (direction * distance), Color.magenta, 2f);
    }

    return _hit;
  }

  static public RaycastHit2D getRaycastRay(Vector2 from, Vector2 direction, float distance, int layer, bool drawDebug = false)
  {
    RaycastHit2D _hit;

    _hit = Physics2D.Raycast(from, direction, distance, layer);
    //Debug.Log(_hit.collider);

    if(drawDebug)
    {
      if (_hit.collider != null) Debug.DrawLine(from, _hit.point, Color.yellow, 2f);
      else Debug.DrawLine(from, from + (direction * distance), Color.magenta, 2f);
    }
    
    return _hit;
  }

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
