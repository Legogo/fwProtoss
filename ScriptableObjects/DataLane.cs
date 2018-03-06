using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "data/new DataLane", order = 100)]
public class DataLane : ScriptableObject {
  
  public DataLanePattern[] patterns;

  public bool randomNextPattern = false;

  public float laneHeight = 1f;
  public float laneSpeed = 1f;

  public DataLaneBuffer bufferEnd;
}
