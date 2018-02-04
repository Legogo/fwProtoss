using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CapacityMovement : LogicCapacity {

  protected Transform _t;
  protected CapacityCollision _collision;
  protected Vector2 lastDirection; // (int) last valid direction
  protected Vector2 lastStep; // previous frame step
  protected Vector2 lastFullMovement; // debug

  protected Vector2 instantForce;
  protected Vector2 velocityForce;
  
  public CapacityPropertyLocker lockHorizontal;
  public CapacityPropertyLocker lockGravity;

  protected float horizontalFrixion = 1f;
  protected bool _moved;
  
  public bool useGravity = true;

  //tools
  protected Vector2 nextPosition;
  
  List<ForceBase> forces = new List<ForceBase>();

  protected override void build()
  {
    base.build();

    _t = transform;
    lockHorizontal = new CapacityPropertyLocker();
    lockGravity = new CapacityPropertyLocker();

    _collision = _owner.GetComponent<CapacityCollision>();
  }

  public override void setupCapacity()
  {
    base.setupCapacity();

    if (useGravity) subscribeToGravity();
  }

  virtual protected void subscribeToGravity()
  {
    forces.Add(new ForceConstant("gravity", Vector2.down));
  }

  public void addForce(ForceBase force) { forces.Add(force); }

  public void addInstant(Vector2 stepForce){ addInstant(stepForce.x, stepForce.y);}
  public void addInstant(float x, float y)
  {
    instantForce.x += x;
    instantForce.y += y;
  }
  
  public void addVelocity(Vector2 force){velocityForce += force;}
  public void addVelocity(float x, float y)
  {
    velocityForce.x += x;
    velocityForce.y += y;
  }

  public override void updateLogic(){
    base.updateLogic();

    int i = 0;
    while(i < forces.Count)
    {
      forces[i].update(); // compute force step
      velocityForce += forces[i].getValue(); // inject value
      if (forces[i].needToBeRemoved())
      {
        //Debug.Log("remove " + forces[i].Name);
        forces.RemoveAt(i); // remove if needed
      }
      else i++;
    }

    //default horizontal frixion
    velocityForce.x = Mathf.MoveTowards(velocityForce.x, 0f, horizontalFrixion);
  }

  public override void updateLogicLate()
  {
    base.updateLogicLate();

    //Debug.Log(Time.frameCount+" , now solving movement for " + name, gameObject);

    //if someone is locking this direction
    int? lockDirection = lockHorizontal.getLockDirection();
    if(lockDirection != null)
    {
      if (Mathf.Sign(instantForce.x) == lockDirection.Value)
      {
        instantForce.x = 0f;
      }
    }

    instantForce += velocityForce;

    Vector3 position = transform.position;

    moveStep(instantForce * Time.fixedDeltaTime);

    lastFullMovement = transform.position - position;

    lastStep = instantForce;

    //direction
    if (instantForce.x != 0f) lastDirection.x = Mathf.Sign(instantForce.x);
    if (instantForce.y != 0f) lastDirection.y = Mathf.Sign(instantForce.y);
    
    instantForce.x = instantForce.y = 0f;
    
    /*
    Debug.Log(Time.frameCount + " end of movement for " + name, gameObject);
    Debug.Log("instant ?  " + instantForce);
    Debug.Log("velocity ? " + velocityForce.x + " x " + velocityForce.y);
    Debug.Log("grounded ? " + isGrounded());
    */

    //AFTER moving
    if (_collision != null)
    {
      if (_collision.isGrounded() || _collision.isRoofed()) killVerticalSpeed();
    }

    //Debug.Log(Time.frameCount + " end of movestep");
  }

  public void killHorizontalSpeed()
  {
    instantForce.x = velocityForce.x = 0f;
  }
  public void killVerticalSpeed()
  {
    //Debug.Log(Time.frameCount+" kill vertical");
    instantForce.y = velocityForce.y = 0f;
  }
  public void killInstantSpeed()
  {
    instantForce = Vector2.zero;
  }

  public bool isGoingUp() { return lastDirection.y > 0f; }
  public bool isFalling() { return lastDirection.y < 0f; }
  public float getVerticalSpeed() { return lastFullMovement.y; }
  public float getHorizontalSpeed() { return lastFullMovement.x; }

  protected void moveStep(Vector2 step)
  {
    Vector2 originOfMovement = _t.position;

    //store for direction
    if (step.x != 0f) lastDirection = step;
    
    //cannot collide
    if (_collision != null && _collision.isCollidable())
    {
      //Debug.Log(name + " , " + step);
      nextPosition = _collision.checkCollisionRaycasts(step);

      /*
      _t.position += (Vector3)step; // apply step move !
      nextPosition = _t.position;
      nextPosition = _collision.checkCollisionRectangle(nextPosition);
      */
    }
    else
    {
      nextPosition = originOfMovement + step;
    }

    _moved = nextPosition != originOfMovement;

    //Debug.DrawLine(_t.position, nextPosition, Color.white);
    //Debug.DrawLine(Vector3.zero, nextPosition, Color.yellow);

    _t.position = nextPosition;
  }

  /* dirige l'entité vers un point */
  public Vector2 moveToward(Vector3 position, float speed)
  {
    Vector2 diff = (position - transform.position);
    Vector2 speedVector = diff.normalized * speed;
    instantForce += speedVector;
    return speedVector;
  }
  
  public override void clean()
  {
    lastDirection.x = 0f;
    lastDirection.y = 0f;
    _lock = false;
    _moved = false;
  }

  public List<ForceBase> getForces() { return forces; }
  
  public int getHorizontalDirection(bool raw = false)
  {
    if (raw) return (int)Mathf.Sign(lastStep.x);
    return (int)lastDirection.x;
  }

  virtual public bool hasMoved()
  {
    return _moved;
  }

  public bool isGrounded() { return (_collision != null) ? _collision.isGrounded() : true; }
  
  public override string toString()
  {
    string ct = base.toString();

    //collision ?
    if (_collision != null) ct += "\n └ " + iStringFormatBool("collidable", _collision.isCollidable());
    else ct += "\n └ " + iStringFormatBool("collision != null", _collision != null);

    ct += "\n └ " + iStringFormatBool("moved", _moved);
    ct += "\n └ movement last frame : " + lastFullMovement.x+" x "+lastFullMovement.y;
    ct += "\n └ position : " + transform.position;

    ct += "\n" + iStringFormatBool("lock horizontal", lockHorizontal.isLocked());

    ct += "\n~solved step data~";
    ct += "\n └ velocity : " + velocityForce.x + " x " + velocityForce.y;
    ct += "\n └ instant : " + instantForce.x + " x " + instantForce.y;
    ct += "\n └ instant : " + lastStep.x + " x " + lastStep.y;
    ct += "\n └ last direction : " + lastDirection.x + " x " + lastDirection.y;

    ct += "\n~forces~";
    for (int i = 0; i < forces.Count; i++)
    {
      ct += "\n   └ " + forces[i].Name + " " + forces[i].getValue().x+"x"+forces[i].getValue().y;
    }

    return ct;
  }
  
}
