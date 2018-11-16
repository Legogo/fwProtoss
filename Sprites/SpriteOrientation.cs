using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///        0°
///        ^
///        |
/// 270° <- -> 90°
///        |
///        v
///       180°
/// 
/// 
/// </summary>

public class SpriteOrientation : MonoBehaviour {

  [Serializable]
  public enum SpriteOrientationDirection { UP, DOWN, LEFT, RIGHT, UP_LEFT, UP_RIGHT, DOWN_LEFT, DOWN_RIGHT };

  [Serializable]
  public struct SpriteOrientationFrame
  {
    public Vector2 degreRange;
    public Sprite frame;
  }

  public Sprite defaultFrame;
  public SpriteOrientationFrame[] frames;

  protected SpriteRenderer spr;

  private void Awake()
  {
    spr = GetComponent<SpriteRenderer>();
    update(Vector2.down);
  }

  public void update(Vector2 dir)
  {
    if (spr == null) return;
    
    spr.sprite = getMatchingAngleFrame(dir);
  }

  public Sprite getMatchingAngleFrame(Vector2 dir)
  {
    return getMatchingAngleFrame(getPositiveAngle(dir));
  }
  public Sprite getMatchingAngleFrame(float angle)
  {
    for (int i = 0; i < frames.Length; i++)
    {
      if (frames[i].degreRange.x < angle && frames[i].degreRange.y > angle) return frames[i].frame;
    }

    if (defaultFrame != null) return defaultFrame;
    return frames[0].frame;
  }

  /// <summary>
  /// return [0,360] based on absolute UP direction and given direction
  /// </summary>
  /// <param name="dir"></param>
  /// <returns></returns>
  protected float getPositiveAngle(Vector2 dir)
  {
    dir.Normalize();

    float angle = Vector2.Angle(Vector2.up, dir);
    //Debug.Log("angle ? "+angle);

    //float updownSign = Vector3.Dot(Vector2.up, dir);
    //Debug.Log("ud ? "+updownSign);

    float leftrightSign = Vector3.Dot(Vector2.right, dir);
    //Debug.Log("lr ? " + leftrightSign);

    float signedAngle = angle * Mathf.Sign(leftrightSign);
    if (signedAngle < 0f) signedAngle = 360f + signedAngle; // make it [0,360]

    //Debug.Log("solved : " + signedAngle);

    return signedAngle;
  }
}

