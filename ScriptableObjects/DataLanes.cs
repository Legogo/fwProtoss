using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "data/new DataLanes", order = 100)]
public class DataLanes : ScriptableObject {

  public DataLaneInfo[] lanes;

  [Serializable]
  public class DataLaneInfo{
    public DataLane lane;
    public int patternWinCount = 1;
  }
}
