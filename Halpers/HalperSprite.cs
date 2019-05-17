using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalperSprite
{
  static public Vector3 getBoundWorldBottomCoord(Bounds bound)
  {
    Vector3 output = bound.center;
    output.y -= bound.extents.y;
    return output;
  }

  static public Vector3 getSpriteWorldBottomCoord(SpriteRenderer spr)
  {
    return getBoundWorldBottomCoord(spr.bounds);
  }
  
  static public Vector3 getSpriteWorldBorderCoord(SpriteRenderer spr, int borderDirection = 1)
  {
    Vector3 output = spr.bounds.center;
    output.x += spr.bounds.extents.x * borderDirection;
    return output;
  }

  static public void drawLineBounds(SpriteRenderer spr)
  {
    Debug.DrawLine(spr.bounds.min, spr.bounds.max);
  }
}
