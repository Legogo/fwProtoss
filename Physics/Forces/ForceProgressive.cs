using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Force that will decrease overtime by an abstract frixion and be removed when == 0f
/// </summary>

public class ForceProgressive : ForceBase
{
  float frixion;

  public ForceProgressive(string name, Vector2 forcePower, float frixionPower = 1f) : base(name, false)
  {
    frixion = frixionPower;
    _force = forcePower;
  }
  
  protected override void compute()
  {
    _force = Vector3.MoveTowards(_force, Vector3.zero, frixion * Time.deltaTime);
  }

  public override bool needToBeRemoved()
  {
    return _force.sqrMagnitude == 0f;
  }
}
