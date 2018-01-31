using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Allow to manage a list of type that will indicates that a specific property is locked
/// </summary>

public class CapacityPropertyLocker {

  List<GameObject> list = new List<GameObject>();

  public void addLock(GameObject locker) { if (!list.Contains(locker)) list.Add(locker); }
  public void removeLock(GameObject locker) { list.Remove(locker); }
  public bool isLocked() { return list.Count > 0; }

}
