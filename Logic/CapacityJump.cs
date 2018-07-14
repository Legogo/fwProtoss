﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

abstract public class CapacityJump : LogicCapacity
{
  protected CapacityCollision _collision;
  protected CapacityMovement _move;

  bool grounded = false;
  bool jumping = false;

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

  public override void updateCapacity()
  {
    base.updateCapacity();

    grounded = _collision.isGrounded();

    if (jumping && grounded)
    {
      //Debug.Log("-------------------------- <b>LAND</b> "+Time.frameCount);
      jumping = false;
    }
    
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
      jumping = true;
      //Debug.Log(" ------------------------ <b>JUMP</b> "+Time.frameCount);
      _move.addVelocity(0f, getJumpPower());

      soundPlayJump();
    }
    
  }
  
  virtual public float getJumpPower()
  {
    return 1f;
  }

  virtual public void soundPlayJump()
  {
    SoundManager.call("PlayerJump");
  }
}
