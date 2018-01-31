using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NOT WORKING
/// </summary>

public class CapacityTopDownMovement : CapacityMovement {

  protected Vector2 direction;
  protected Vector2 solvedDirection; // [0,1]
  protected Vector2 lastDirection;

  protected float progressiveSpeed = 0f;
  protected float clampSpeed = 1f;

  public bool lockPosition = false;
  public bool concentrating = false;
  
  InputKeyTopDown _inputLocal;

  GameSpace _gameSpace;

  protected override void build()
  {
    base.build();
    lastDirection = Vector2.right;
  }

  public override void setupCapacity()
  {
    _inputLocal = _owner.GetComponent<CapacityInput>().keys.get<InputKeyTopDown>() as InputKeyTopDown;
    _gameSpace = GameSpace.get();
  }

  public CapacityTopDownMovement setup(float progressiveSpeed, float clampSpeed)
  {
    this.progressiveSpeed = progressiveSpeed;
    this.clampSpeed = clampSpeed;
    return this;
  }
  
  public override void updateLogic()
  {
    direction.x = direction.y = 0f;

    lockPosition = Input.GetKey(KeyCode.LeftControl);

    if (_inputLocal.pressing_up()) direction.y = 1f;
    else if (_inputLocal.pressing_down()) direction.y = -1f;

    if (_inputLocal.pressing_left()) direction.x = -1f;
    else if (_inputLocal.pressing_right()) direction.x = 1f;

    if (progressiveSpeed == 0f)
    {
      solvedDirection = direction;
    }
    else
    {
      solvedDirection.x = Mathf.MoveTowards(solvedDirection.x, direction.x, Time.deltaTime * progressiveSpeed);
      solvedDirection.y = Mathf.MoveTowards(solvedDirection.y, direction.y, Time.deltaTime * progressiveSpeed);
      solvedDirection = Vector2.ClampMagnitude(solvedDirection, clampSpeed);
    }

    if (direction.sqrMagnitude > 0f) lastDirection = direction;
    
    //move
    if (!lockPosition)
    {
      _owner.transform.Translate(getDirection());
    }

    //clamp in screen
    if (_gameSpace != null) _owner.forceWithinBounds(_gameSpace.offsetSpace);
  }
  
  public Vector2 getLastDirection() { return lastDirection; }
  public Vector2 getDirection() { return solvedDirection; }
  
  public override void clean()
  {
  }

  public override float getGravityPower()
  {
    return 0f;
  }
}
