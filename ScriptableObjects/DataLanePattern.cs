using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "data/new DataLanePattern", order = 100)]
public class DataLanePattern : ScriptableObject
{
  public Sprite[] obstacles;
  public float spawnTimer = 3f;

  public float patternFactorSpeed = 1f;

}
