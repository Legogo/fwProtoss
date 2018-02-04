using System;
using UnityEngine;

public class ForceGravity : ForceBase
{
  PlayerLogic owner;
  //HiddenCapacityWallGrab wallgrab;

  public ForceGravity(PlayerLogic player) : base("gravity", false)
  {
    owner = player;
    //wallgrab = owner.GetComponent<HiddenCapacityWallGrab>();
  }

  protected override void compute()
  {
    //if (wallgrab.isSnapped()) _force.y = -owner.getParam().movement.wallGravityClamp;
    _force.y = -owner.getParam().movement.gravityFactor;
  }
}
