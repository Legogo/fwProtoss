using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

/// <summary>
/// https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html
/// https://docs.unity3d.com/ScriptReference/Networking.DownloadHandler.html
/// text	Convenience property. Returns the bytes from data interpreted as a UTF8 string. (Read Only)
/// </summary>

static public class HalperWeb {
  
  static public IEnumerator query(string url, Action<string> onComplete, Action onTimeout, float timeout = 3f)
  {
    UnityWebRequest www = new UnityWebRequest(url);

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

    if (onComplete != null) onComplete(www.downloadHandler.text);
  }
}
