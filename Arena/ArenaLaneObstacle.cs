using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class ArenaLaneObstacle : ArenaObject {
  
  protected float speed = 1f;
  
  virtual public void setupOnLane(ArenaLane lane)
  {
    speed = lane.data.obstacleTranslateSpeed;

    if (lane.data.obstacleSpeedRange.sqrMagnitude != 0f)
    {
      speed = Random.Range(lane.data.obstacleSpeedRange.x, lane.data.obstacleSpeedRange.y);
    }

    visibility.show();
  }

  public float getSpeed() { return speed; }

  abstract public void onTouched(ArenaObject other, Collider2D otherColliderTouched, string collisionType);
}
