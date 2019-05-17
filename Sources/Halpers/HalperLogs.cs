using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static  public class HalperLogs {

  static public string gatherDataHierarchyInfo(Transform tr)
  {
    string ct = "";
    if (tr.parent != null) ct = tr.parent + "/" + tr.name;
    else ct = tr.name;
    ct += " (children " + tr.childCount + ") " + tr.gameObject.activeSelf;

    ct += "[";
    Component[] comps = tr.GetComponents<Component>();
    for (int i = 0; i < comps.Length; i++)
    {
      ct += "," + comps[i].GetType();
    }
    ct += "]";

    foreach(Transform child in tr)
    {
      ct += "\n  L "+ gatherDataHierarchyInfo(child);
    }

    return ct;
  }

}
