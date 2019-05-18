using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "protoss/create ScriptableTimerParams", order = 100)]
public class ScriptableTimerParams : ScriptableObject
{
  public float timer = 1f;
  public Vector2 rangeTimer;

  public float getTimeTarget() { return timer; }

  public float getRandomTimeTarget()
  {
    if(rangeTimer.sqrMagnitude != 0f)
    {
      return Random.Range(rangeTimer.x, rangeTimer.y);
    }
    return getTimeTarget();
  }
}
