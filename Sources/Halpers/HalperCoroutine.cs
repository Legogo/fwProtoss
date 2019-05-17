using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperCoroutine {

  static public IEnumerator processTimer(float time)
  {

    //yield return new WaitForSeconds(2f);
    while (time > 0f)
    {
      time -= Time.deltaTime;
      yield return null;
    }

  }

  /// <summary>
  /// to run a list of enumerator until all are done running
  /// </summary>
  /// <param name="processList"></param>
  /// <returns></returns>
  static public IEnumerator processProcess(List<IEnumerator> processList)
  {
    while(processList.Count > 0)
    {
      int idx = 0;
      while(idx < processList.Count)
      {
        if (!processList[idx].MoveNext()) processList.RemoveAt(idx);
        else idx++;
      }

      //Debug.Log("  L process count : " + processList.Count);
      yield return null;
    }
  }
}
