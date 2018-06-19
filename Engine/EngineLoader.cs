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
  static protected EngineLoader loader;
  protected List<AsyncOperation> _asyncs = new List<AsyncOperation>();

  public Action onLoadingDone;
  
  protected bool SHOW_DEBUG = false;
  string prefix = "resource-";
  
  [RuntimeInitializeOnLoadMethod]
  static protected void init()
  {
    loader = GameObject.FindObjectOfType<EngineLoader>();
    if (loader != null) return; // already init

#if UNITY_EDITOR
    Debug.Log("<color=gray><b>~EngineLoader~</b> | app entry point</color>");
#endif

    //don't load engine on guide scenes (starting with ~)
    if(SceneManager.GetActiveScene().name.StartsWith("~")) {
      Debug.LogWarning("<b>guide scene</b> not loading engine here");
      return;
    }
    
    loader = new GameObject("[loader]").AddComponent<EngineLoader>();
  }

  static protected bool checkForFilteredScenes()
  {
    string[] filter = { "screen", "resource", "level" };
    for (int i = 0; i < filter.Length; i++)
    {
      if (isSceneOfName(filter[i]))
      {
        //SceneManager.LoadScene("game");
        Debug.LogWarning("<color=red><b>" + filter[i] + " SCENE ?!</b></color> can't load that");
        return false;
      }

    }
    return true;
  }
  
  IEnumerator Start()
  {
    if (SHOW_DEBUG) Debug.Log("start of <color=green>system loading</color> ...");
    
    ///// first we load engine, to get the feeder script
    loadScene(prefix+"engine");
    while (!allAsyncDone()) yield return null;

    Debug.Log("<color=gray>~EngineLoader~ engine scene is done loading</color>");

    //NEEDED
    EngineManager.create();

    ///// feeder, additionnal scenes (from feeder script)
    EngineLoaderFeeder feeder = EngineLoaderFeeder.get();
    List<string> all = new List<string>();
    if (feeder != null) all.AddRange(feeder.feed());

    string debugContent = "~EngineLoader~ now loading <b>" + all.Count + " scenes</b> ... ";
    for (int i = 0; i < all.Count; i++) debugContent += "\n  " + all[i];
    Debug.Log(debugContent);

    ///// now wait for feeder scenes to load
    for (int i = 0; i < all.Count; i++) loadScene(all[i]);
    while (!allAsyncDone()) yield return null;
    
    doneLoading();
  }

  bool allAsyncDone()
  {
    if (_asyncs.Count > 0) return false;
    return true;
  }

  void doneLoading() {

    Debug.Log("~EngineLoader~ ... done loading!");

    if (onLoadingDone != null) onLoadingDone();
    
    GameObject.DestroyImmediate(gameObject);
  }
  
  void loadScene(string sceneLoad)
  {
    //Debug.Log(SceneManager.sceneCountInBuildSettings);

    if(SceneManager.sceneCountInBuildSettings <= 1)
    {
      Debug.LogWarning("could not launch loading of " + sceneLoad + " because build settings scenes is <b>empty</b>");
      return;
    }
    
    //do not load the current active scene
    if (!isSceneOfName(sceneLoad))
    {
      StartCoroutine(process_loadScene(sceneLoad));
    }
  }

  IEnumerator process_loadScene(string sceneLoad)
  {

    //can't reload same scene
    //if (isSceneOfName(sceneLoad)) yield break;

    //don't double load same scene
    if (SceneManager.GetSceneByName(sceneLoad).isLoaded) yield break;
    
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
  
  static public EngineLoader get() {
    if (loader == null) init();
    return loader;
  }
}
