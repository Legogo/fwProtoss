using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CapacityJump : LogicCapacity
{
  CapacityCollision _collision;
  CapacityMovement _move;

  InputKeyTopDown _inputKey;

  public Action onJump;

  public override void setupCapacity()
  {
    _collision = _owner.GetComponent<CapacityCollision>();
    _move = _owner.GetComponent<CapacityMovement>();

    if (_owner.input.keys != null) _inputKey = _owner.input.keys.get<InputKeyTopDown>();
    _owner.input.touch += touch; // mouse input
  }

  protected void touch(InputTouchFinger finger)
  {
    solveJump();
  }

  public override void updateLogic()
  {
    base.updateLogic();

    if(_inputKey != null)
    {
      //Debug.Log(_inputKey.pressed_jump());
      if (_inputKey.pressed_jump()) solveJump();
    }
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
