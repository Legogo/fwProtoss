using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// attach this script to a MonoBehavior to be able to create a circle of collider on it
/// </summary>

public class HalperMonoCollisionsMakeCircle : MonoBehaviour {

  public LayerMask collisionLayer;

  [ContextMenu("make circle")]
  protected void makeCircle()
  {
    Collider[] existing = transform.GetComponentsInChildren<Collider>();

    int qty = Mathf.Max(20, existing.Length);
    float width = 3f;
    Collider refCollider = transform.GetChild(0).GetComponent<Collider>();

    List<Collider> all = new List<Collider>();
    all.AddRange(existing);

    Collider colid;

    while (all.Count < qty)
    {
      colid = qh.dupl<Collider>(refCollider.gameObject);
      colid.transform.SetParent(transform);
      all.Add(colid);
    }

    float radius = 7.5f;
    float step = (Mathf.PI * 2f) / all.Count;
    Vector3 pos;
    
    float angleStep = step * Mathf.Rad2Deg;

    for (int i = 0; i < all.Count; i++)
    {

      pos.x = Mathf.Cos(step * i) * radius;
      pos.z = Mathf.Sin(step * i) * radius;
      pos.y = 0f;

      all[i].transform.localScale = Vector3.one + (Vector3.forward * width) + (Vector3.up * 2f);
      all[i].transform.position = pos;
      all[i].transform.rotation = Quaternion.AngleAxis(i * angleStep, -Vector3.up);
      all[i].name = "collider-circle-" + Mathf.FloorToInt(angleStep * i);
      //all[i].gameObject.layer = 1 << collisionLayer.value;
      all[i].gameObject.layer = HalperLayers.ToLayer(collisionLayer.value);
    }
  }

}
