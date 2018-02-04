using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager {
  
  static protected List<GameObject> resources = new List<GameObject>();

  /* called by EngineManager::engine_scenes_loaded */
  static public void reload()
  {
    GameObject[] gos = GameObject.FindGameObjectsWithTag("resource");

    resources.Clear();

    //kill all unused
    for (int i = 0; i < gos.Length; i++)
    {
      if (gos[i].name.StartsWith("~")) GameObject.DestroyImmediate(gos[i].gameObject);
      else resources.Add(gos[i]);
    }

    Debug.Log("~ResMa~ <b>" + resources.Count + " resource(s)</b> loaded");
    for (int i = 0; i < resources.Count; i++)
    {
      //Debug.Log("    - " + resources[i].name, resources[i].gameObject);
      resources[i].gameObject.SetActive(false);
    }
    
  }
  
  static public GameObject getResourceByName(string nm)
  {
    for (int i = 0; i < resources.Count; i++)
    {
      if (resources[i].name.StartsWith(nm)) return resources[i];
    }
    return null;
  }

  static public T getDuplicate<T>(string nm, string rename = "")
  {
    GameObject obj = getDuplicate(nm, rename);
    if (obj == null)
    {
      Debug.LogWarning("no object found in resources named : <b>" + nm+"</b>");
      return default(T);
    }

    T comp = obj.GetComponent<T>();
    if (comp == null) comp = obj.GetComponentInChildren<T>();
    return comp;
  }

  static public GameObject getDuplicate(string nm, string rename = "")
  {
    GameObject obj = getResourceByName(nm);
    if (obj == null) return null;
    obj = GameObject.Instantiate(obj);
    if(rename.Length > 0) obj.name = rename;
    obj.SetActive(true);
    return obj;
  }
}
