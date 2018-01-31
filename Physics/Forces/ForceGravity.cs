using System;
using UnityEngine;

public class ForceGravity : ForceBase
{
  float gravityStrength = 0f;

  public ForceGravity(float strength) : base("gravity", false)
  {
    gravityStrength = strength;
  }

  protected override void compute()
  {
    _force.y = -gravityStrength;
  }
}

public class AttractionForce : ForceBase
{
  private Vector3 _point;
  private float forceStrength;

  public AttractionForce(Vector2 point, float force, string name) : base(name, false)
  {
    _point = point;
    forceStrength = force;
  }

  protected override void compute()
  {
    _force = (_point - _movement.transform.position) * forceStrength;
  }
}
