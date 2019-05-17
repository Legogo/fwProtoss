using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class PprefsManager {

  const string ppref_uniq = "ppref_uniq";

  static public string uniq = "";

  [RuntimeInitializeOnLoadMethod]
  static public void create()
  {
    uniq = ppGet(ppref_uniq, HalperNatives.generateUniqId());
  }

  static public void ppSet(string id, string val)
  {
    PlayerPrefs.SetString(id, val);
    PlayerPrefs.Save();
  }
  static public string ppGet(string id, string defaultValue = "")
  {
    return PlayerPrefs.GetString(id, defaultValue);
  }
}
