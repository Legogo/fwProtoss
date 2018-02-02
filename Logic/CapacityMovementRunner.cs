using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapacityMovementRunner : CapacityMovement
{

  public override void updateLogic()
  {
    base.updateLogic();

    addForce(1f, 0f);
  }

  public override float getGravityPower()
  {
    return 1f;
  }
}
