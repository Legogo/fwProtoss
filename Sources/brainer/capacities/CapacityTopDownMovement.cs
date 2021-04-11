using UnityEngine;

namespace brainer.capacity
{
  public class CapacityTopDownMovement : CapacityMovement
  {
    protected Vector2 direction;
    protected Vector2 solvedDirection; // [0,1]

    protected float progressiveSpeed = 0f;
    protected float clampMagnitudeSpeed = 1f;

    public float moveSpeed = 1f;
    public bool useDefaultInput = true;

    //InputKeyTopDown _inputLocal;
    Vector2 move = Vector2.zero;

    GameSpace _gameSpace;

    public override void setupCapacity()
    {
      lastDirection = Vector2.right;

      setup(0f, 1f);

      if (useDefaultInput)
      {
        //_inputLocal = _owner.input.get<InputKeyTopDown>();
      }
      
      //Debug.Log(_inputLocal + " " + _owner.name, transform);

      _gameSpace = GameSpace.get();
    }

    public CapacityTopDownMovement setup(float progressiveSpeed, float clampSpeed)
    {
      this.progressiveSpeed = progressiveSpeed;
      this.clampMagnitudeSpeed = clampSpeed;
      return this;
    }

    public override void updateCapacity()
    {
      //use of force and velocity ?
      base.updateCapacity(); // this was disabled ... 

      //direction.x = direction.y = 0f;

      direction = getInputMovement();

      //Debug.Log(direction);

      if (progressiveSpeed == 0f)
      {
        solvedDirection = direction;
      }
      else
      {
        solvedDirection.x = Mathf.MoveTowards(solvedDirection.x, direction.x, progressiveSpeed);
        solvedDirection.y = Mathf.MoveTowards(solvedDirection.y, direction.y, progressiveSpeed);
        solvedDirection = Vector2.ClampMagnitude(solvedDirection, clampMagnitudeSpeed);
      }

      if (direction.sqrMagnitude > 0f) lastDirection = direction;

      instantVelocity += solvedDirection * moveSpeed;
      
      //clamp in screen
      //if (_gameSpace != null) brain.forceWithinBounds(_gameSpace.offsetSpace);
    }
    
    virtual protected Vector2 getInputMovement()
    {
      move.x = move.y = 0f;
      
      /*
      if(_inputLocal != null)
      {
        if (_inputLocal.pressing_up()) move.y = 1f;
        else if (_inputLocal.pressing_down()) move.y = -1f;

        if (_inputLocal.pressing_left()) move.x = -1f;
        else if (_inputLocal.pressing_right()) move.x = 1f;
      }
      */

      return move;
    }
    
  }

}
