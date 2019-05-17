using UnityEngine;
using fwp.input;

/// <summary>
/// NOT WORKING
/// </summary>

namespace fwp
{
  public class CapacityTopDownMovement : CapacityMovement
  {

    protected Vector2 direction;
    protected Vector2 solvedDirection; // [0,1]

    protected float progressiveSpeed = 0f;
    protected float clampMagnitudeSpeed = 1f;

    public float moveSpeed = 1f;

    InputKeyTopDown _inputLocal;

    GameSpace _gameSpace;

    protected override void build()
    {
      base.build();
      lastDirection = Vector2.right;

      setup(0f, 1f);
    }

    public override void setupCapacity()
    {
      _inputLocal = _owner.input.get<InputKeyTopDown>();
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
      //direction.x = direction.y = 0f;

      direction = getInputMovement();

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
      if (_gameSpace != null) _owner.forceWithinBounds(_gameSpace.offsetSpace);
    }

    virtual protected Vector2 getInputMovement()
    {
      Vector2 move = Vector2.zero;

      if (_inputLocal.pressing_up()) move.y = 1f;
      else if (_inputLocal.pressing_down()) move.y = -1f;

      if (_inputLocal.pressing_left()) move.x = -1f;
      else if (_inputLocal.pressing_right()) move.x = 1f;

      return move;
    }

    public Vector2 getLastDirection() { return lastDirection; }
    public Vector2 getDirection() { return solvedDirection; }

    public override void clean()
    {
    }

  }

}
