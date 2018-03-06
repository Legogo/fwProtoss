﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

abstract public class CapacityJump : LogicCapacity
{
  CapacityCollision _collision;
  CapacityMovement _move;
  
  public Action onJump;

  public override void setupCapacity()
  {
    _collision = _owner.GetComponent<CapacityCollision>();
    _move = _owner.GetComponent<CapacityMovement>();
    
    _owner.input.touch += touch; // mouse input
  }

  abstract protected bool pressJump();

  protected void touch(InputTouchFinger finger)
  {
    solveJump();
  }

  public override void updateLogic()
  {
    base.updateLogic();

    if(pressJump()) solveJump();
  }

  [ContextMenu("jump!")]
  public void solveJump()
  {
    bool isGrounded = _collision.isGrounded();

    //Debug.Log("<color=red>================================================</color>");
    //Debug.Log(Time.frameCount+" , <b>JUMP !</b> grounded ? "+isGrounded + " , jump pwr ? " + getJumpPower());

    if (onJump != null) onJump();

    if (isGrounded)
    {
      _move.addVelocity(0f, getJumpPower());
      
      SoundManager.call("PlayerJump");
    }
    
  }
  
  virtual public float getJumpPower()
  {
    return 1f;
  }
}