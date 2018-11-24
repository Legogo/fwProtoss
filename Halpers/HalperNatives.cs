﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

static public class HalperNatives {


  /// <summary>
  /// yyyy-mm-dd_hh:mm
  /// </summary>
  static public string getFullDate()
  {
    DateTime dt = DateTime.Now;
    return dt.Year + "-" + dt.Month + "-" + dt.Day + "_" + dt.Hour + "-" + dt.Minute;
  }

  static public string getFrDate()
  {
    DateTime dt = DateTime.Now;
    return dt.Day + "-" + dt.Month + "-" + dt.Year;
  }

  static public string getNowHourMin(char separator = ':')
  {
    DateTime dt = DateTime.Now;

    string hour = (dt.Hour< 10) ? "0" + dt.Hour: dt.Hour.ToString();
    string min = (dt.Minute < 10) ? "0"+dt.Minute : dt.Minute.ToString();

    return hour + separator + min;
  }
  
  static public void os_openFolder(string folderPath)
  {
    folderPath = folderPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
    System.Diagnostics.Process.Start("explorer.exe", "/select," + folderPath);
  }


  //https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
  /// <summary>
  /// Windows Store Apps: Application.persistentDataPath points to %userprofile%\AppData\Local\Packages\<productname>\LocalState.
  /// iOS: Application.persistentDataPath points to /var/mobile/Containers/Data/Application/<guid>/Documents.
  /// Android: Application.persistentDataPath points to /storage/emulated/0/Android/data/<packagename>/files on most devices
  /// </summary>
  /// <returns></returns>
  static public string getDataPath()
  {
    return Application.persistentDataPath;
  }

}