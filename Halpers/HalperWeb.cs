using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

static public class HalperWeb {
  
  static public IEnumerator query(string url, Action<string> onComplete, Action onTimeout, float timeout = 3f)
  {
    WWW www = new WWW(url);

    while (!www.isDone && timeout > 0f)
    {
      timeout -= Time.deltaTime;
      yield return null;
    }

    if(timeout <= 0f)
    {
      if (onTimeout != null) onTimeout();
      yield break;
    }

    if (onComplete != null) onComplete(www.text);
  }
}
