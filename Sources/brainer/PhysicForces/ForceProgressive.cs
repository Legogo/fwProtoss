using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Force that will decrease overtime by an abstract frixion and be removed when == 0f
/// </summary>

namespace brainer
{
  public class ForceProgressive : ForceBase
  {
    Vector2 originPower;
    float frixion;

    public ForceProgressive(string name, Vector2 forcePower, float frixionPower = 1f) : base(name, false)
    {
      originPower = forcePower;
      frixion = frixionPower;
      _force = forcePower;
    }

    protected override void compute()
    {
      _force = Vector3.MoveTowards(_force, Vector3.zero, frixion * Time.deltaTime);
    }

    public float getDeltaWithOrigin()
    {
      return Mathf.Abs(originPower.magnitude - _force.magnitude);
    }
    public float getDeltaWithOriginProgress()
    {
      return getDeltaWithOrigin() / originPower.magnitude;
    }

    public override bool needToBeRemoved()
    {
      return _force.sqrMagnitude == 0f;
    }
  }

}
