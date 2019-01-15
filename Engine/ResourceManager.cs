using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
      //you might need to have a guide canvas to work on UI visuals
      //instaed of removing it (default behavior with guides objects) we just deactivate it
      Canvas cv = gos[i].GetComponentInParent<Canvas>();

      if(cv != null && cv.name.StartsWith("~")) cv.enabled = false; // kill canvas guide of resource object

      if (gos[i].name.StartsWith("~")) GameObject.Destroy(gos[i].gameObject); // guides objects
      else resources.Add(gos[i]); // normal resource object

    }

    string debugContent = "~ResMa~ <b>" + resources.Count + " resource(s)</b> loaded ("+gos.Length+" initially found)";
    for (int i = 0; i < resources.Count; i++)
    {
      debugContent += "\n    - " + resources[i].name;
      resources[i].gameObject.SetActive(false);
    }

    if (debugContent.Length > 0) Debug.Log(debugContent);
  }
  
  /// <summary>
  /// returned the original object
  /// </summary>
  static public GameObject getResourceByName(string nm)
  {
    for (int i = 0; i < resources.Count; i++)
    {
      if (resources[i].name.StartsWith(nm)) return resources[i];
    }
    return null;
  }

  static public GameObject[] getAllResourceByPrefix(string prefix)
  {
    List<GameObject> objs = new List<GameObject>();
    for (int i = 0; i < resources.Count; i++)
    {
      if (resources[i].name.StartsWith(prefix)) objs.Add(resources[i]);
    }
    return objs.ToArray();
  }

  static public bool hasResourceByName(string nm)
  {
    for (int i = 0; i < resources.Count; i++)
    {
      if (resources[i].name.StartsWith(nm)) return true;
    }
    return false;
  }

  /// <summary>
  /// will create a duplicate of resource element of given name, 
  /// remove parent if parent is canvas.
  /// <b>Object is deactivated by default !</b>
  /// </summary>
  static public T getDuplicate<T>(string nm, string rename = "") where T : Component
  {
    if(nm.Length <= 0)
    {
      Debug.LogError("getDuplicate : resource to duplicate seek 'name' is empty : you need to provide a name to find a resource");
      return null;
    }
    
    GameObject obj = getDuplicate(nm, rename);
    if (obj == null)
    {
      Debug.LogWarning("no object found in resources named : '<b>" + nm+"</b>'");
      return default(T);
    }
    
    //Debug.Log("duplicate resource of name : " + nm);

    T comp = obj.GetComponent<T>();

    //get canvas of origin resource
    GameObject go = getResourceByName(nm);
    Canvas cs = go.GetComponentInParent<Canvas>();

    //setup new generated object child of canvas
    if(cs != null)
    {
      obj.transform.SetParent(go.transform.parent);

      //Debug.Log("ResourceManager :: canvas item :: "+ obj.name + " is child of " + go.name + " parent : " + go.transform.parent.name, obj.transform);
    }
    //else Debug.LogWarning("no canvas for " + nm, obj.transform);

    if (comp == null) comp = obj.GetComponentInChildren<T>();
    return comp;
  }
  
  static public GameObject getDuplicate(string nm, string rename = "")
  {
    GameObject obj = getResourceByName(nm);
    if (obj == null) return null;
    obj = GameObject.Instantiate(obj);
    if(rename.Length > 0) obj.name = rename;
    obj.tag = "Untagged"; // remove resource tag !
    obj.SetActive(true);
    return obj;
  }
}
