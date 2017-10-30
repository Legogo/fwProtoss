using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System;

public class CheckInternet
{

  static public bool hasInternet = false;

  static public void checkInternet(Action<bool> onCheckDone)
  {
    string HtmlText = GetHtmlFromUri("http://google.com");
    
    Debug.Log("GetHtmlFromUri(http://google.com), RAW HTML --->\n"+HtmlText);
    
    hasInternet = false;

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
      hasInternet = true;
    }

    #if no_internet
      hasInternet = false;
    #endif

    //Debug.Log("INTERNET ? " + hasInternet);
    if (onCheckDone != null) onCheckDone(hasInternet);
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
 