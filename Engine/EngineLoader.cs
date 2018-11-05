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

  protected List<Coroutine> queries = new List<Coroutine>();
  
  public Action onLoadingDone;
  
  string prefix = "resource-";
  
  [RuntimeInitializeOnLoadMethod]
  static protected void init()
  {
#if UNITY_EDITOR
    Debug.Log("<color=gray><b>~EngineLoader~</b> | app entry point</color>");
#endif
    
    loader = create();

    loader.startupProcess();
  }

  static protected EngineLoader create()
  {
    loader = GameObject.FindObjectOfType<EngineLoader>();
    if (loader != null) return loader;

    loader = new GameObject("[loader]").AddComponent<EngineLoader>();
    return loader;
  }

  static protected bool checkForFilteredScenes()
  {
    string[] filter = { "ui", "screen", "resource", "level" };
    for (int i = 0; i < filter.Length; i++)
    {
      if (isActiveSceneName(filter[i]))
      {
        //SceneManager.LoadScene("game");
        Debug.LogWarning("<color=red><b>" + filter[i] + " SCENE ?!</b></color> can't load that");
        return false;
      }
    }
    return true;
  }
  
  static protected bool canLoad()
  {

    if (SceneManager.sceneCountInBuildSettings <= 1)
    {
      Debug.LogWarning("could not launch loading because build settings scenes list is <b>empty</b>");
      return false;
    }

    return true;
  }
  
  public void startupProcess()
  {

    //don't load engine on guide scenes (starting with ~)
    if (SceneManager.GetActiveScene().name.StartsWith("~"))
    {
      Debug.LogWarning("<b>guide scene</b> not loading engine here");
      return;
    }

    if (!canLoad())
    {
      Debug.Log(getStamp()+"can't load ?");
    }

    Coroutine co = null;
    co = StartCoroutine(processStartup(delegate()
    {
      evtQueryDone(co);
    }));
    queries.Add(co);
  }
  
  IEnumerator processStartup(Action onComplete = null)
  {
    Coroutine co = null;
    
    ///// then we load engine, to get the feeder script
    co = loadScenes(new string[] { prefix + "engine" });
    while(queries.IndexOf(co) > -1) yield return null;
    
    //NEEDED
    EngineManager.create();

    Scene sc = SceneManager.GetActiveScene();
    cleanScene(sc);

    ///// feeder, additionnal scenes (from feeder script)
    EngineLoaderFeeder[] feeders = GameObject.FindObjectsOfType<EngineLoaderFeeder>();

    List<string> all = new List<string>();
    for (int i = 0; i < feeders.Length; i++)
    {
      if (feeders[i] != null) all.AddRange(feeders[i].feed());
    }

    //string debugContent = "~EngineLoader~ now loading <b>" + all.Count + " scenes</b> ... ";
    //for (int i = 0; i < all.Count; i++) debugContent += "\n  " + all[i];

    co = loadScenes(all.ToArray());
    while (queries.IndexOf(co) > -1) yield return null;

    if (onComplete != null) onComplete();
  }
  
  public void evtQueryDone(Coroutine co)
  {
    queries.Remove(co);

    //Debug.Log(queries.Count + " queries left");

    evtSceneIsDoneLoading();
  }

  void evtSceneIsDoneLoading() {
    //Debug.Log("a query is done , " + queries.Count + " left");

    if(queries.Count > 0)
    {
      return;
    }

    Debug.Log(getStamp()+" ... done loading!");

    if (onLoadingDone != null) onLoadingDone();
    
    GameObject.DestroyImmediate(gameObject);
  }
  
  public Coroutine loadScenes(string[] sceneNames, Action onComplete = null)
  {
    Coroutine co = null;

    //Debug.Log(getStamp() + "loadScenes[" + sceneNames.Length + "]");

    co = StartCoroutine(processLoadScenes(sceneNames, delegate()
    {
      //Debug.Log(" a query (of "+sceneNames.Length+") is done");

      evtQueryDone(co);

      if(onComplete != null) onComplete();
    }));

    queries.Add(co);

    //Debug.Log("added query for " + sceneNames.Length + " scenes to load ("+queries.Count+" total)");

    return co;
  }
  
  IEnumerator processLoadScenes(string[] sceneNames, Action onComplete = null)
  {
    //Debug.Log("  ... processing " + sceneNames.Length + " scenes");

    for (int i = 0; i < sceneNames.Length; i++)
    {
      string sceneName = sceneNames[i];

      //do not load the current active scene
      if (isActiveSceneName(sceneName))
      {
        Debug.LogWarning("trying to load active scene ?");
        continue;
      }

      //don't double load same scene
      if (SceneManager.GetSceneByName(sceneName).isLoaded)
      {
        Debug.LogWarning(sceneName + " is concidered as already loaded");
        continue;
      }
      
      IEnumerator process = processLoadScene(sceneNames[i]);
      while (process.MoveNext()) yield return null;

      //Debug.Log("  ... scene of index " + i + " | "+sceneNames[i]+" | is done loading");
    }
    
    //needed so that all new objects loaded have time to exec build()
    yield return null;

    //Debug.Log("  ... processing " + sceneNames.Length + " is done");

    if (onComplete != null) onComplete();
  }

  IEnumerator processLoadScene(string sceneLoad, Action onComplete = null)
  {
    //can't reload same scene
    //if (isSceneOfName(sceneLoad)) yield break;

    Debug.Log(getStamp() + "  L <b>"+sceneLoad+"</b> loading ... ");

    AsyncOperation async = SceneManager.LoadSceneAsync(sceneLoad, LoadSceneMode.Additive);
    while (!async.isDone)
    {
      yield return null;
      //Debug.Log(sceneLoad + " "+async.progress);
    }

    //Debug.Log(getStamp() + "  L <b>" + sceneLoad + "</b> async is done ... ");

    Scene sc = SceneManager.GetSceneByName(sceneLoad);
    while (!sc.isLoaded) yield return null;

    //Debug.Log(getStamp() + "  L <b>" + sceneLoad + "</b> at loaded state ... ");

    cleanScene(sc);

    //Debug.Log(getStamp()+" ... '<b>" + sceneLoad + "</b>' loaded");

    if (onComplete != null) onComplete();
  }
  
  protected string getStamp()
  {
    return "<color=gray>"+GetType()+"</color> | ";
  }



  static public Coroutine queryScene(string sceneName, Action onComplete = null)
  {
    return queryScenes(new string[] { sceneName }, onComplete);
  }
  static public Coroutine queryScenes(string[] sceneNames, Action onComplete = null)
  {
    return get().loadScenes(sceneNames, onComplete);
  }

  static protected void cleanScene(Scene sc)
  {

    GameObject[] roots = sc.GetRootGameObjects();
    //Debug.Log("  L cleaning scene <b>" + sc.name + "</b> from guides objects (" + roots.Length + " roots)");
    for (int i = 0; i < roots.Length; i++)
    {
      removeGuides(roots[i].transform);
    }
    
  }

  static protected bool removeGuides(Transform obj)
  {
    if(obj.name.StartsWith("~"))
    {
      Debug.Log("   removing guide object : " + obj.name);
      GameObject.DestroyImmediate(obj.gameObject);
      return true;
    }

    int i = 0;
    while(i < obj.childCount)
    {
      if (!removeGuides(obj.GetChild(i))) i++;
    }

    return false;
  }

  static protected string getLevelName() {
    return SceneManager.GetActiveScene().name;
  }
  
  static public bool isActiveSceneName(string nm) {
    return getLevelName().Contains(nm);
  }

  static protected bool isResourceScene()
  {
    return isActiveSceneName("resource-");
  }

  static protected bool isSceneLevel()
  {
    return isActiveSceneName("level-");
  }
  
  static public EngineLoader get() {
    if (loader == null) create();
    return loader;
  }

  static public bool isLoading()
  {
    return loader != null;
  }

  static public bool isSceneInBuildSettingsList(string scName)
  {
    bool found = true;

#if UNITY_EDITOR

    found = false;

    UnityEditor.EditorBuildSettingsScene[] scenes = UnityEditor.EditorBuildSettings.scenes;
    for (int i = 0; i < scenes.Length; i++)
    {
      //UnityEditor.SceneManagement.EditorSceneManager.GetSceneByBuildIndex()
      if (scenes[i].path.Contains(scName)) found = true;
    }
    
#endif

    return found;
  }
}
