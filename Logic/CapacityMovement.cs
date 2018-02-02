using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CapacityMovement : LogicCapacity {

  protected Transform _t;
  protected CapacityCollision _collision;
  protected Vector2 lastInstantForce; // backup
  protected Vector2 lastFullMovement;

  protected Vector2 instantForce;
  protected Vector2 velocityForce;

  public CapacityPropertyLocker lockHorizontal;
  public CapacityPropertyLocker lockGravity;

  protected bool _moved;

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

  public int getHorizontalDirection()
  {
    return (int)Mathf.Sign(lastInstantForce.x);
  }
  
  protected void subscribeToGravity()
  {
    forces.Add(new ForceGravity(getGravityPower()));
  }

  public void addForce(Vector2 stepForce)
  {
    addForce(stepForce.x, stepForce.y);
    
  }
  public void addForce(float x, float y)
  {
    instantForce.x += x;
    instantForce.y += y;
  }
  public void addForce(ForceBase force)
  {
    forces.Add(force);
  }

  public void addVelocity(Vector2 force)
  {
    velocityForce += force;
  }
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
      if (forces[i].needToBeRemoved()) forces.RemoveAt(i); // remove if needed
      else i++;
    }
  }

  public override void updateLogicLate()
  {
    base.updateLogicLate();

    instantForce += velocityForce;

    //Debug.Log(name + " = " + instantForce);
    Vector3 position = transform.position;
    moveStep(instantForce * Time.deltaTime);
    lastFullMovement = transform.position - position;

    lastInstantForce = instantForce; //Save last
    instantForce.x = instantForce.y = 0f;
    
    if (_collision != null && _collision.isGrounded()) killVerticalSpeed();
  }
  
  public void killHorizontalSpeed()
  {
    instantForce.x = velocityForce.x = 0f;
  }
  public void killVerticalSpeed()
  {
    instantForce.y = velocityForce.y = 0f;
  }
  public void killInstantSpeed()
  {
    instantForce = Vector2.zero;
  }

  public bool isGoingUp() { return lastInstantForce.y > 0f; }
  public bool isFalling() { return lastInstantForce.y < 0f; }
  public float getVerticalSpeed() { return lastInstantForce.y; }
  public float getHorizontalSpeed() { return lastInstantForce.x; }

  protected void moveStep(Vector2 step)
  {
    Vector2 originOfMovement = _t.position;

    //store for direction
    if (step.x != 0f) lastInstantForce = step;

    Debug.DrawLine(transform.position + (Vector3.down * 0.2f), transform.position + (Vector3.down * 0.2f) + ((Vector3)step * 5f), Color.black);

    //Debug.Log(name + " , " + step);

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
    lastInstantForce.x = 0f;
    lastInstantForce.y = 0f;
    _lock = false;
    _moved = false;
  }

  public List<ForceBase> getForces() { return forces; }
  
  virtual public bool hasMoved()
  {
    return _moved;
  }

  public bool isGrounded() { return (_collision != null) ? _collision.isGrounded() : true; }

  abstract public float getGravityPower();

  public override string toString()
  {
    string ct = base.toString();

    if (_collision != null) ct += "\n └ " + iStringFormatBool("collidable ?", _collision.isCollidable());

    
    ct += "\n └ position : " + transform.position;
    ct += "\n └ moved ? " + _moved;
    ct += "\n └ movement last frame : " + lastFullMovement.x+" x "+lastFullMovement.y;

    ct += "\n~forces~";

    ct += "\n └ velocity : " + velocityForce.x + " x " + velocityForce.y;
    ct += "\n └ instant : " + instantForce.x + " x " + instantForce.y;
    ct += "\n └ last instant : " + lastInstantForce.x + " x " + lastInstantForce.y;

    return ct;
  }
  
}
