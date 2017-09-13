using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class EngineLoader : MonoBehaviour
{
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
    
    //cam
    StartCoroutine(process_loadScene("resource-engine"));
    StartCoroutine(process_loadScene("resource-sound"));
    
    StartCoroutine(waitForAsyncs(doneLoading));
  }

  IEnumerator waitForAsyncs(Action onDone = null)
  {
    //Debug.Log(_asyncs.Count + " asyncs loading");

    while (_asyncs.Count > 0) yield return null;
    if(onDone != null) onDone();
  }

  void doneLoading() {
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
  
  protected string getLevelName() { return SceneManager.GetActiveScene().name; }
  protected bool isSceneOfName(string nm) { return getLevelName().Contains(nm); }
  protected bool isSceneLevel()
  {
    return isSceneOfName("level-");
  }

  static public EngineLoader _instance;
}
