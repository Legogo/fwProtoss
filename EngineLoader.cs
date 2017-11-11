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
  protected List<AsyncOperation> _asyncs;

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

  void Awake()
  {
    _asyncs = new List<AsyncOperation>();
  }

  void Start()
  {
    call_loading_system();
  }

  //importer tout ce qui va servir au jeu
  protected void call_loading_system()
  {
    if (SHOW_DEBUG) Debug.Log("start of <color=green>system loading</color> ...");
    
    List<string> all = new List<string>();

    string prefix = "resource-";
    all.Add(prefix+"engine");
    all.Add(prefix+"sound");

    string[] names = new string[0];

    //additionnal scenes
    EngineLoaderFeeder feeder = EngineLoaderFeeder.get();
    if(feeder != null)
    {
      names = feeder.feed();
    }
    
    all.AddRange(names);

    //merge
    names = all.ToArray();

    //start async
    for (int i = 0; i < names.Length; i++)
    {
      if (!isSceneOfName(names[i])) StartCoroutine(process_loadScene(names[i]));
    }

    StartCoroutine(waitForAsyncs(doneLoading));
  }

  IEnumerator waitForAsyncs(Action onDone = null)
  {
    //Debug.Log(_asyncs.Count + " asyncs loading");

    while (_asyncs.Count > 0) yield return null;

    //NEED a frame for all Start to process
    yield return null;

    if(onDone != null) onDone();
  }

  void doneLoading() {

    EngineManager em = EngineManager.get();
    if (em != null) em.engine_scenes_loaded();
    
    GameObject.DestroyImmediate(gameObject);
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
