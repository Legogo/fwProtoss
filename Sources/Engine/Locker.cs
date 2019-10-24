using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker
{
  List<MonoBehaviour> locks = new List<MonoBehaviour>();

  public Locker()
  {

  }

  public void remove<T>() where T : MonoBehaviour
  {
    int i = 0;
    while(i < locks.Count)
    {
      if(locks[i] as T)
      {
        locks.RemoveAt(i);
        continue;
      }

      i++;
    }
  }

  public bool has(MonoBehaviour obj)
  {
    return locks.IndexOf(obj) > -1;
  }

  public void add(MonoBehaviour obj)
  {
    if (locks.IndexOf(obj) < 0) locks.Add(obj);
  }

  public void remove(MonoBehaviour obj)
  {
    if (locks.IndexOf(obj) > -1) locks.Remove(obj);
  }

  public bool isLocked()
  {
    return locks.Count > 0;
  }
}
