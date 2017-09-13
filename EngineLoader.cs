using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class EngineLoader : MonoBehaviour
{
/*
  protected enum eLoadingStates { BUILD = 0, LOADING = 1, IDLE = 2 };
  protected eLoadingStates _state = eLoadingStates.BUILD;

  protected List<AsyncOperation> _asyncs;

  protected bool SHOW_DEBUG = false;

  [RuntimeInitializeOnLoadMethod]
  static protected void init()
  {
    Debug.Log("<color=gray>Engine entry point</color>");

    new GameObject("[loader]").AddComponent<EngineLoader>();
  }

  void Awake()
  {
    _instance = this;
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

    if (isSceneLevel())
    {
      new GameObject("(debug)").AddComponent<DebugManager>();
      //new GameObject("(game)").AddComponent<GameState>();
    }

    //cam
    StartCoroutine(process_loadScene("resource-engine"));
    StartCoroutine(process_loadScene("resource-sounds"));

    StartCoroutine(process_loadScene("resource-avatar"));

    if (isSceneLevel())
    {
      //player
      StartCoroutine(process_loadScene("resource-level"));
    }

    StartCoroutine(waitForAsyncs(setup));
  }

  IEnumerator waitForAsyncs(Action onDone)
  {
    //Debug.Log(_asyncs.Count + " asyncs loading");

    while (_asyncs.Count > 0) yield return null;
    onDone();
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


  //un setup global quand les resources sont dispos (sans level)
  void setup()
  {

    EngineObject[] items = GameObject.FindObjectsOfType<EngineObject>();
    for (int i = 0; i < items.Length; i++)
    {
      items[i].loading_systemDone();
    }

    if (SHOW_DEBUG) Debug.Log("... <b>system setup</b> loading done");

    GameState game = GameObject.FindObjectOfType<GameState>();
    if (game != null) game.restartLevel();
  }

  protected string getLevelName() { return SceneManager.GetActiveScene().name; }
  protected bool isSceneOfName(string nm) { return getLevelName().Contains(nm); }
  protected bool isSceneLevel()
  {
    return isSceneOfName("level-");
  }

  static public EngineLoader _instance;
  static public bool isDoneLoading()
  {
    return _instance._state >= eLoadingStates.IDLE;
  }
  */
}
