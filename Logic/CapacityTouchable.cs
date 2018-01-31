using System;
using System.Linq;
using UnityEngine;

public class CapacityTouchable : LogicCapacity
{
  public Collider2D targetCollider;

  public override void setupCapacity()
  {
    targetCollider = GetComponent<Collider2D>();
  }
  public override void updateLogic()
  {
  }

  public bool Completed()
  {
    return false;
  }

  public bool CollideWith(Collider2D playerCollider)
  {
    Collider2D[] results = new Collider2D[10];
    targetCollider.OverlapCollider(new ContactFilter2D(), results);
    foreach (var result in results.Where(x => x != null))
    {
      if (result == playerCollider)
      {
        return true;
      }
    }
    return false;
  }

  public override void clean()
  {
  }
}
