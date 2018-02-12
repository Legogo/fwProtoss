using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "dino/new DataLane", order = 100)]
public class DataLane : ScriptableObject {

  public float laneHeight = 1f;
  public Sprite[] obstacles;
  public DataTimer timer;

  public Vector2 obstacleSpeedRange;
  public float obstacleTranslateSpeed = 1f;

}
