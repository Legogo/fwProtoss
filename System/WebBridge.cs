using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

/// <summary>
/// https://docs.unity3d.com/ScriptReference/WWW.html
/// https://docs.unity3d.com/ScriptReference/WWWForm.html
/// </summary>

public class WebBridge : MonoBehaviour {
  
  static public WebBridge get()
  {
    WebBridge wb = qh.gc<WebBridge>();
    if(wb == null)
    {
      GameObject obj = HalperGameObject.getGameObject("[web]");
      wb = obj.AddComponent<WebBridge>();
    }
    return wb;
  }

  public void ping(Action<bool> onComplete)
  {
    StartCoroutine(processPing("http://www.andreberlemont.com/ping.php", onComplete));
  }

  IEnumerator processPing(string url, Action<bool> onComplete)
  {
    IEnumerator query = HalperWeb.query(url, delegate (string answer)
    {
      onComplete(true);
    },
    delegate ()
    {
      onComplete(false);
    });

    while (query.MoveNext()) yield return null;
  }

  public UnityWebRequest query(string url, Action<string> onComplete, params string[] datas)
  {
    WWWForm form = new WWWForm();

    for (int i = 0; i < datas.Length; i+=2)
    {
      form.AddField(datas[i], datas[i + 1]);
    }

    UnityWebRequest uwr = UnityWebRequest.Post(url, form);
    StartCoroutine(processQuery(uwr, onComplete));
    return uwr;
  }

  IEnumerator processQuery(UnityWebRequest uwr, Action<string> onComplete)
  {
    Debug.Log("sending query : " + uwr.url);

    yield return uwr.SendWebRequest();

    //while (!uwr.isDone) yield return null;

    if (onComplete != null) onComplete(uwr.downloadHandler.text);
  }

  public void query(string url, Action<string> onComplete, Action onTimeout, float timeout = 3f)
  {
    StartCoroutine(HalperWeb.query(url, onComplete, onTimeout, timeout));
  }
}
