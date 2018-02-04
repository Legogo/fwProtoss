using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that allow to gather information about direction of CharacterLogic owner
/// And right stick angle (based on direction)
/// 
/// solve : direction, angle
/// sends : onAimPress, onAimRelease virtuals on matching events
/// </summary>

abstract public class CapacityAim : LogicCapacity
{
  //protected InputKeyAim _inputAim;

  CapacityMovement _move;
  protected float _angle = 0f;
  
  public override void setupCapacity() {
    if (_character == null) Debug.LogError(name+" need character logic", gameObject);

    _move = _owner.GetComponent<CapacityMovement>();
    //CapacityInput ci = _character.GetComponent<CapacityInput>();
    //_inputAim = ci.keys.get<InputKeyAim>();
  }
  
  public override void updateLogic() {
    _angle = getRightStickAngle();
    
    if (pressed_aim()) onAimPress();
    else if (release_aim()) onAimRelease();
  }

  abstract protected void onAimPress();
  abstract protected void onAimRelease();

  abstract protected Vector2 getJoystickVector();
  abstract protected bool pressed_aim();
  abstract protected bool release_aim();

  public float getRightStickAngle()
  {

    float angle = _move.getHorizontalDirection() < 0 ? 0f : 180f;
    Vector2 joyVector = getJoystickVector();
    if(joyVector.sqrMagnitude != 0f)
    {
      angle = -ComputeSignedAngle(new Vector2(-1, 0), joyVector);
    }

    return angle;
  }

  public float ComputeSignedAngle(Vector3 from, Vector3 to)
  {
    var angle = Vector3.Angle(from, to);
    var sign = Vector3.Cross(from, to);
    if (sign.y < 0) angle = -angle;
    return angle;
  }
  
  public override void updateLogicLate() { }
  public override void clean(){}
}
