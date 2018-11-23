using System.Collections;
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
    return dt.Hour + "" + separator + dt.Minute;
  }

}
