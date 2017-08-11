using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateCrasher : MonoBehaviour {

  public float iterations = 1000;
  public bool lagActive = false;
#if UNITY_EDITOR

  protected Coroutine coProcess;

  protected bool press()
  {
    return (Input.GetKeyUp(KeyCode.RightShift));
  }

  private void Update()
  {
    if(press())
    {
      if (coProcess != null) return;
      coProcess = StartCoroutine(processLag());
    }
  }

  IEnumerator processLag()
  {
    lagActive = true;

    yield return null;

    bool flag = false;
    
    while(!flag)
    {

      if (press()) flag = true;

      for (int i = 0; i < iterations; i++)
      {
        GameObject.FindObjectsOfType<GameObject>();
      }

      yield return null;
    }

    lagActive = false;

    coProcess = null;
  }

#endif

}
