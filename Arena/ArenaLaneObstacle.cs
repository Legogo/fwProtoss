using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

abstract public class ArenaLaneObstacle : ArenaObject {

  public float laneSpeed = 1f;
  protected float solvedSpeed = 1f;
  protected Collider2D[] _colliders;
  
  abstract public void fetchColliders();

  virtual public void setupOnLane(ArenaLane lane)
  {
    fetchColliders();

    solvedSpeed = laneSpeed * lane.data.laneFactorSpeed;
    
    visibility.show();
  }
  
  public void checkOutOfScreen()
  {

    //remove on ooscreen
    if (transform.position.x < -12f)
    {
      GameObject.DestroyImmediate(gameObject);
    }
  }

  public Collider2D checkCollision(Collider2D refCollider)
  {
    for (int i = 0; i < _colliders.Length; i++)
    {
      if (_colliders[i].bounds.Intersects(refCollider.bounds))
      {
        return _colliders[i];
      }
    }

    return null;
  }
  
  public float getSpeed() { return solvedSpeed; }

  virtual public bool canBeCollidedWith()
  {
    return true;
  }

  /* will lane movement will be applied to object */
  virtual public bool canTranslate()
  {
    return true;
  }

  //abstract public void onTouched(ArenaObject other, Collider2D otherColliderTouched);
}
