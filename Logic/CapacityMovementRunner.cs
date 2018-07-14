using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapacityMovementRunner : CapacityMovement
{
  public float gravityPower = 1f;
  public float forwardMovingSpeed = 1f;

  public override void setupCapacity()
  {
    base.setupCapacity();
    if (useGravity) subscribeToGravity();
  }

  public override void updateCapacity()
  {
    base.updateCapacity();

    addInstant(forwardMovingSpeed, 0f); // use joystick to move horizontaly
  }

  public override string toString()
  {
    string ct = base.toString();
    ct += "\n  speed : " + forwardMovingSpeed;
    return ct;
  }
}
