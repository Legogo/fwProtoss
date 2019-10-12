using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// create
/// load engine
/// wait for feeders & co
/// destroy
/// </summary>

public class EngineStartup : MonoBehaviour
{
  static public bool compatibility = false; // permet de savoir si le moteur est actif

  static protected EngineLoader loader;

  public Action onLoadingDone;

  [RuntimeInitializeOnLoadMethod]
  static protected void init()
  {
#if UNITY_EDITOR
    Debug.Log("<color=gray><b>~"+typeof(EngineStartup)+"~</b> | app entry point</color>");
#endif

    string filter = isContextEngineCompatible();
    if (filter.Length > 0)
    {
      Debug.LogWarning("won't load engine here : scene starts with prefix : <b>" + filter + "</b>");

      //EngineManager.create();

      return;
    }

    compatibility = true;

    new GameObject("[startup]").AddComponent<EngineStartup>().startupProcess();
  }
  
  public void startupProcess()
  {

    //don't load engine on guide scenes (starting with ~)
    if (EngineLoader.doActiveSceneNameContains("~", true))
    {
      Debug.LogWarning("<color=red><b>guide scene</b> not loading engine here</color>");
      return;
    }

    if (!EngineLoader.hasAnyScenesInBuildSettings())
    {
      Debug.Log(getStamp() + "can't load ?");
    }

    StartCoroutine(processStartup());
  }

  IEnumerator processStartup()
  {
    Coroutine co = null;

    Debug.Log(getStamp() + " process startup, frame : " + Time.frameCount);

    //leave a few frame for loading screen to be created and displayed
    yield return null;
    yield return null;
    yield return null;

    //Debug.Log(getStamp() + " loading screen should be visible, frame : " + Time.frameCount);

    // then we load engine, to get the feeder script
    co = EngineLoader.loadScenes(new string[] { EngineLoader.prefixResource + "engine" }, 
      delegate(){ co = null; });

    while(co != null) yield return null;

    //NEEDED if not present
    //must be created after the (existing ?) engine scene is loaded (doublon)
    EngineManager.create();

    Debug.Log(getStamp()+" is done at frame "+Time.frameCount+", removing gameobject");
    
    //tant qu'on a des loaders qui tournent ...
    while (EngineLoader.areAnyLoadersRunning()) yield return null;

    yield return null;

    if (onLoadingDone != null) onLoadingDone();

    GameObject.Destroy(gameObject);
  }

  static public string isContextEngineCompatible()
  {
    string[] filters = new string[] { "#", "network" };

    for (int i = 0; i < filters.Length; i++)
    {
      if (EngineLoader.doActiveSceneNameContains(filters[i], true))
      {
        return filters[i];
      }
    }

    return "";
  }

  string getStamp()
  {
    return EngineObject.getStamp(this);
  }

  static public bool instanceExist()
  {
    return GameObject.FindObjectOfType<EngineStartup>() != null;
  }
}
