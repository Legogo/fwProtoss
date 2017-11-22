using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System;

public class CheckInternet
{

  static protected bool _hasInternet = false;
  static public bool hasInternet() { return _hasInternet; }
  
  protected const float timeout = 1f;
  static public string info = "";
  static protected float startTime = 0f;
  static protected float elapsed = 0f;

  static public void checkPing(Action<bool> onCheckDone)
  {
    Debug.Log("!!ping!! checkPing");
    CoroutineInfo info = CoroutineManager.launch(CheckConnection(onCheckDone));
    info.context = "CheckInternet";
  }

  static protected IEnumerator CheckConnection(Action<bool> onCheckDone)
  {
    startTime = Time.timeSinceLevelLoad;
    Ping ping = new Ping("8.8.8.8"); // google dns

    elapsed = 0f;

    while (true)
    {
      info = "Checking network...";

      elapsed = Time.timeSinceLevelLoad - startTime;

      if (ping.isDone)
      {
        info = "Network available.";

        onPingEnded(onCheckDone, true);
        
        yield break;
      }

      if (elapsed > timeout)
      {
        info = "No network.";

        onPingEnded(onCheckDone, false);
        
        yield break;
      }

      yield return new WaitForEndOfFrame();
    }

  }

  static protected void onPingEnded(Action<bool> endCallback, bool state)
  {
    _hasInternet = state;

    Debug.Log("!!ping!! ping ended | info ? "+info+" | internet ? "+ _hasInternet + " | started at " + startTime + " , elapsed : " + elapsed);

    if (endCallback != null) endCallback(state);
  }



  static public void checkInternet(Action<bool> onCheckDone)
  {
    string HtmlText = GetHtmlFromUri("http://google.com");
    
    Debug.Log("GetHtmlFromUri(http://google.com), RAW HTML --->\n"+HtmlText);
    
    _hasInternet = false;

    if (HtmlText == "")
    {
      //No connection
    }
    else if (!HtmlText.Contains("schema.org/WebPage"))
    {
      //Redirecting since the beginning of googles html contains that 
      //phrase and it was not found
    }
    else
    {
      //success
      _hasInternet = true;
    }

#if no_internet
      Debug.Log("!!ping!! scriptable symbol no_internet force no internet state");
      hasInternet = false;
#endif

    onPingEnded(onCheckDone, _hasInternet);
  }

  static protected string GetHtmlFromUri(string resource)
  {
    string html = string.Empty;
    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);

    req.Timeout = 1000;
    //req.ReadWriteTimeout = 1000;

    try
    {
      using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
      {
        bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
        if (isSuccess)
        {
          using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
          {
            //We are limiting the array to 80 so we don't have
            //to parse the entire html document feel free to 
            //adjust (probably stay under 300)
            char[] cs = new char[80];
            reader.Read(cs, 0, cs.Length);
            foreach (char ch in cs)
            {
              html += ch;
            }
          }
        }
      }
    }
    catch
    {
      return "";
    }
    return html;
  }

}
 