using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Allow to manage a list of type that will indicates that a specific property is locked
/// 
/// 0 == both direction
/// 1 == can't move right
/// -1 == can't move left
/// 
/// null == free
/// </summary>

public class CapacityPropertyLocker {

  List<GameObject> list = new List<GameObject>();
  List<int> direction = new List<int>();

  public void addLock(GameObject locker, int dir = 0)
  {
    if (!list.Contains(locker))
    {
      list.Add(locker);
      direction.Add(dir);
    }
  }
  public void removeLock(GameObject locker) {
    int idx = list.IndexOf(locker);
    list.RemoveAt(idx);
    direction.RemoveAt(idx);
  }

  public bool isLocked() { return list.Count > 0; }
  public int? getLockDirection()
  {
    bool left = false;
    bool right = false;
    for (int i = 0; i < list.Count; i++)
    {
      if (direction[i] < 0) left = true;
      if (direction[i] > 0) right = true;
    }
    if (left && !right) return -1;
    if (right && !left) return 1;
    if (left && right) return 0;
    return null;
  }
}
