using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperPprefs {
  
  static public void clearPprefs()
  {
    PlayerPrefs.DeleteAll();
    PlayerPrefs.Save();

    Debug.Log("all pprefs deleted");
  }

}
