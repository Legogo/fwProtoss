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

}
