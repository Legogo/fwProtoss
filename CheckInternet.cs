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
  static public string info = "";

  static public void checkPing(Action<bool> onCheckDone)
  {
    CoroutineInfo info = CoroutineManager.launch(CheckConnection(onCheckDone));
    info.context = "CheckInternet";
  }

  static public IEnumerator CheckConnection(Action<bool> onCheckDone)
  {
    const float timeout = 3f;
    float startTime = Time.timeSinceLevelLoad;
    Ping ping = new Ping("8.8.8.8"); // google dns

    while (true)
    {
      info = "Checking network...";
        
      if (ping.isDone)
      {
        _hasInternet = true;
        info = "Network available.";

        Debug.Log("((ping)) " + info);
        if (onCheckDone != null) onCheckDone(true);
        yield break;
      }

      if (Time.timeSinceLevelLoad - startTime > timeout)
      {
        info = "No network.";
        _hasInternet = false;

        Debug.Log("((ping)) "+info);
        if (onCheckDone != null) onCheckDone(false);
        yield break;
      }

      yield return new WaitForEndOfFrame();
    }
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
      hasInternet = false;
    #endif

    //Debug.Log("INTERNET ? " + hasInternet);
    if (onCheckDone != null) onCheckDone(_hasInternet);
  }

  static public string GetHtmlFromUri(string resource)
  {
    string html = string.Empty;
    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
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
 