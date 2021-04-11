using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalperPhysic {

  static public Rect getRect(Rect bounds, BoxCollider c)
  {
    bounds.width = c.bounds.extents.x * 2f;
    bounds.height = c.bounds.extents.z * 2f;
    bounds.x = c.transform.position.x - bounds.width * 0.5f;
    bounds.y = c.transform.position.z - bounds.height * 0.5f;
    return bounds;
  }

  static public Rect getRect(Rect bounds, Transform tr)
  {

    bounds.width = tr.localScale.x;
    bounds.height = tr.localScale.z;

    bounds.x = tr.position.x - bounds.width * 0.5f;
    bounds.y = tr.position.z - bounds.height * 0.5f;

    return bounds;
  }

  //permet de savoir si deux objets se croisent
  static public bool touchXY(Rect a, Rect b)
  {

    if (a.width == 0f || a.height == 0f)
    {
      Debug.LogWarning("<CollisionTools> object A has a size of 0f");
      return false;
    }
    if (b.width == 0f || b.height == 0f)
    {
      Debug.LogWarning("<CollisionTools> object B has a size of 0f");
      return false;
    }

    Vector2 gap = (b.center - a.center);
    gap.x = Mathf.Abs(gap.x);
    gap.y = Mathf.Abs(gap.y);

    if (gap.x < a.width * 0.5f + b.width * 0.5f)
    {
      if (gap.y < a.height * 0.5f + b.height * 0.5f)
      {
        return true;
      }
    }

    return false;
  }

  static public float rayX(Rect a, Rect b)
  {
    float gap = b.center.x - a.center.x;
    float size = (a.width * 0.5f + b.width * 0.5f);
    if (Mathf.Abs(gap) > size) return 0f;
    return (Mathf.Abs(gap) - size) * Mathf.Sign(gap);
  }

  static public float rayY(Rect a, Rect b)
  {
    float gap = b.center.y - a.center.y;
    float size = (a.height * 0.5f + b.height * 0.5f);

    if (Mathf.Abs(gap) > size) return 0f;
    return (Mathf.Abs(gap) - size) * Mathf.Sign(gap);
  }

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
    RaycastHit _hit;

    Debug.DrawLine(from, from + (direction * distance), Color.magenta, 2f);

    if (Physics.Raycast(from, direction, out _hit, distance, 1 << layer))
    {
      Debug.DrawLine(from, _hit.point, Color.yellow, 2f);
      return _hit.point;
    }
    return Vector2.one * Mathf.Infinity;
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
