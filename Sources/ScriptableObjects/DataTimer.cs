using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "protoss/create DataTimer", order = 100)]
public class DataTimer : ScriptableObject
{
  public float timer = 1f;
  public Vector2 rangeTimer;

  public float fetchTime()
  {
    if(rangeTimer.sqrMagnitude != 0f)
    {
      return Random.Range(rangeTimer.x, rangeTimer.y);
    }
    return timer;
  }
}
