using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

abstract public class ArenaLaneObstacle : ArenaObject {
  
  protected Collider2D[] _colliders;
  
  abstract public void fetchColliders();

  virtual public void setupOnLane()
  {
    fetchColliders();
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

  static public void cleanAllObstacles()
  {
    ArenaLaneObstacle[] obstacles = GameObject.FindObjectsOfType<ArenaLaneObstacle>();

    //clean obstacles
    while (obstacles.Length > 0)
    {
      GameObject.DestroyImmediate(obstacles[0].gameObject);
      //obstacles.RemoveAt(0);
    }
      
  }
}
