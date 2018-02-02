using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapacityMovementRunner : CapacityMovement
{
  
  public float forwardMovingSpeed = 1f;

  [Header("gravity stuff")]
  public bool useGravity = true;
  public float gravityPower = 1f;

  public override void setupCapacity()
  {
    base.setupCapacity();
    if (useGravity) subscribeToGravity();
  }

  public override void updateLogic()
  {
    base.updateLogic();

    addForce(forwardMovingSpeed, 0f);
  }

  public override float getGravityPower()
  {
    return gravityPower;
  }

  public override string toString()
  {
    string ct = base.toString();
    ct += "\n  speed : " + forwardMovingSpeed;
    return ct;
  }
}
