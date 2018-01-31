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
  protected InputKeyAim _inputAim;

  protected int _direction = 0;
  protected float _angle = 0f;
  
  public override void setupCapacity() {
    if (_character == null) Debug.LogError(name+" need character logic", gameObject);

    CapacityInput ci = _character.GetComponent<CapacityInput>();
    _inputAim = ci.keys.get<InputKeyAim>();
  }
  
  public override void updateLogic() {
    _direction = _character.Direction;
    _angle = getRightStickAngle();
    
    if (_inputAim.pressed_aim()) onAimPress();
    else if (_inputAim.pressed_release()) onAimRelease();
  }

  abstract protected void onAimPress();
  abstract protected void onAimRelease();
  
  public float getRightStickAngle()
  {

    float angle = _direction < 0 ? 0f : 180f;

    XinputController controller = _inputAim.getXinput();

    if (controller != null)
    {
      if (controller.rightStickVector != Vector3.zero)
      {
        angle = -ComputeSignedAngle(new Vector2(-1, 0), controller.rightStickVector);
      }
      else if (controller.leftStickVector != Vector3.zero)
      {
        angle = -ComputeSignedAngle(new Vector2(-1, 0), controller.leftStickVector);
      }
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
