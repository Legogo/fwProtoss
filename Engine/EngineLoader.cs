using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// BUILD
/// (loading)
/// SETUP
/// (check)
/// LIVE
/// </summary>

public class EngineLoader : MonoBehaviour
{
  protected List<AsyncOperation> _asyncs = new List<AsyncOperation>();

  protected bool SHOW_DEBUG = false;

  [RuntimeInitializeOnLoadMethod]
  static protected void init()
  {
    #if UNITY_EDITOR
    Debug.Log("<color=gray><b>Engine</b> entry point</color>");
#endif

    //only load in game scene
    string[] filter = { "screen", "resource", "level" };
    for (int i = 0; i < filter.Length; i++)
    {
      if (isSceneOfName(filter[i]))
      {
        //SceneManager.LoadScene("game");
        Debug.LogWarning("<color=red><b>"+filter[i]+" SCENE ?!</b></color> can't load that");
        return;
      }

    }

    new GameObject("[loader]").AddComponent<EngineLoader>();
  }
  
  IEnumerator Start()
  {
    if (SHOW_DEBUG) Debug.Log("start of <color=green>system loading</color> ...");


    yield return null;

    List<string> all = new List<string>();

    //default scenes
    string prefix = "resource-";
    all.Add(prefix+"engine");
    all.Add(prefix+"sound");

    ///// first, we load the basic engine dependencies
    for (int i = 0; i < all.Count; i++) loadScene(all[i]);
    while (_asyncs.Count > 0) yield return null;

    all.Clear();

    ///// feeder, additionnal scenes (from feeder script)
    EngineLoaderFeeder feeder = EngineLoaderFeeder.get();
    if(feeder != null) all.AddRange(feeder.feed());
    
    ///// now wait for feeder scenes to load
    for (int i = 0; i < all.Count; i++) loadScene(all[i]);
    while (_asyncs.Count > 0) yield return null;

    //NEED multiples frames for all Start to process
    yield return null;
    yield return null;
    yield return null;

    all.Clear();

    doneLoading();
  }

  void doneLoading() {

    EngineManager em = EngineManager.get();
    if (em != null) em.engine_scenes_loaded();
    
    GameObject.DestroyImmediate(gameObject);
  }
  
  void loadScene(string sceneLoad)
  {
    //do not load the current active scene
    if (!isSceneOfName(sceneLoad))
    {
      StartCoroutine(process_loadScene(sceneLoad));
    }
  }

  IEnumerator process_loadScene(string sceneLoad)
  {

    //can't reload same scene
    if (isSceneOfName(sceneLoad)) yield break;

    AsyncOperation async = SceneManager.LoadSceneAsync(sceneLoad, LoadSceneMode.Additive);
    _asyncs.Add(async);

    //Debug.Log("  package '<b>" + sceneLoad + "</b>' | starting loading");

    while (!async.isDone) yield return null;

    _asyncs.Remove(async);

    if (SHOW_DEBUG) Debug.Log("  package '<b>" + sceneLoad + "</b>' | done loading (" + _asyncs.Count + " left)");
  }
  
  static protected string getLevelName() {
    return SceneManager.GetActiveScene().name;
  }
  
  static public bool isSceneOfName(string nm) {
    return getLevelName().Contains(nm);
  }

  static protected bool isResourceScene()
  {
    return isSceneOfName("resource-");
  }

  static protected bool isSceneLevel()
  {
    return isSceneOfName("level-");
  }
  
}
