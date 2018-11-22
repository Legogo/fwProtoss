using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class GlobalSettingsMobile
{

  static public void setupSonyAndroid()
  {

    //sony xperia as a software home menu we have to force "not fullscreen" to keep buttons visible
    //some ad plugin doesn't manage alignment of bottom banner to home menu so it makes it float not anchored to the bottom

    string[] models = { "sony" };

    Debug.Log("models : " + SystemInfo.deviceModel + " checking for fullscreen removal");

    for (int i = 0; i < models.Length; i++)
    {
      //skip
      if (!Screen.fullScreen) continue;

      Debug.Log("models : " + SystemInfo.deviceModel + " VS " + models[i]);

      if (SystemInfo.deviceModel.ToLower().Contains(models[i].ToLower()))
      {
        Debug.Log("models : " + SystemInfo.deviceModel + " is set for NOT fullscreen");
        Screen.fullScreen = false;
      }
    }

  }

}
