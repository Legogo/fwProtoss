using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

abstract public class CapacityJump : LogicCapacity
{
  CapacityCollision _collision;
  HiddenCapacityMovement _move;

  InputKeyTopDown _input;

  public override void setupCapacity()
  {
    _collision = _owner.GetComponent<CapacityCollision>();
    _move = _owner.GetComponent<HiddenCapacityMovement>();
    _input = _owner.GetComponent<CapacityInput>().keys.get<InputKeyTopDown>();
  }

  public override void updateLogic()
  {
    //jump when snapped on wall
    if (_input.pressed_jump())
    {
      solveJump();
    }
  }
  
  protected void solveJump()
  {
    bool isGrounded = _collision.isGrounded();
    
    if (isGrounded)
    {
      _move.addVelocity(0f, getJumpPower());
      SoundManager.call("PlayerJump");
    }
    
  }
  
  abstract public float getJumpPower();
}
