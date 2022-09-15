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

namespace fwp.engine.scaffolder.pocEngine
{
    public class EngineLoader : MonoBehaviour
    {
        static public bool compatibility = false; // permet de savoir si le moteur est actif

        static protected EngineLoader loader;

        protected List<Coroutine> queries = new List<Coroutine>();

        public Action onLoadingDone;

        string prefix = "resource-";

        //[RuntimeInitializeOnLoadMethod]
        static public void init()
        {
#if UNITY_EDITOR
            Debug.Log("<color=gray><b>~EngineLoader~</b> | app entry point</color>");
#endif

            string filter = isContextEngineCompatible();
            if (filter.Length > 0)
            {
                Debug.LogWarning("won't load engine here : scene starts with prefix : <b>" + filter + "</b>");
                return;
            }

            compatibility = true;

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
                if (doActiveSceneNameContains(filter[i]))
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
                Debug.LogWarning("could not launch loading because <b>build settings scenes list count <= 1</b>");
                return false;
            }

            return true;
        }

        public void startupProcess()
        {

            //don't load engine on guide scenes (starting with ~)
            if (SceneManager.GetActiveScene().name.StartsWith("~"))
            {
                Debug.LogWarning("<color=red><b>guide scene</b> not loading engine here</color>");
                return;
            }

            if (!canLoad())
            {
                Debug.Log(getStamp() + "can't load ?");
            }

            Coroutine co = null;
            co = StartCoroutine(processStartup(delegate ()
            {
                evtQueryDone(co);
            }));

            queries.Add(co);
        }

        IEnumerator processStartup(Action onComplete = null)
        {
            Coroutine co = null;

            //leave a few frame for loading screen to be created and displayed
            yield return null;
            yield return null;
            yield return null;

            ///// then we load engine, to get the feeder script
            co = loadScenes(new string[] { prefix + "engine" });
            while (queries.IndexOf(co) > -1) yield return null;

            //NEEDED
#if poc
            EngineManager.create();
#endif

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

        void evtSceneIsDoneLoading()
        {
            //Debug.Log("a query is done , " + queries.Count + " left");

            if (queries.Count > 0)
            {
                return;
            }

            Debug.Log(getStamp() + " ... done loading!");

            if (onLoadingDone != null) onLoadingDone();

            GameObject.Destroy(gameObject);
        }

        public Coroutine loadScenes(string[] sceneNames, Action onComplete = null)
        {
            Coroutine co = null;

            //Debug.Log(getStamp() + "loadScenes[" + sceneNames.Length + "]");

            co = StartCoroutine(processLoadScenes(sceneNames, delegate ()
            {
                //Debug.Log(" a query (of "+sceneNames.Length+") is done");

                evtQueryDone(co);

                if (onComplete != null) onComplete();
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
                if (doActiveSceneNameContains(sceneName))
                {
                    Debug.LogWarning("trying to load active scene ?");
                    continue;
                }

                //don't double load same scene
                if (SceneManager.GetSceneByName(sceneName).isLoaded)
                {
                    Debug.LogWarning("<b>" + sceneName + "</b> is considered as already loaded");
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

            if (!checkIfCanBeLoaded(sceneLoad))
            {
                Debug.LogWarning("asked to load <b>" + sceneLoad + "</b> but this scene is <b>not added to BuildSettings</b>");
                yield break;
            }

#if UNITY_EDITOR
            Debug.Log(getStamp() + "  L <b>" + sceneLoad + "</b> loading ... ");
#endif

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
            return "<color=gray>" + GetType() + "</color> | ";
        }



        static public Coroutine queryScene(string sceneName, Action onComplete = null)
        {
            return queryScenes(new string[] { sceneName }, onComplete);
        }
        static public Coroutine queryScenes(string[] sceneNames, Action onComplete = null)
        {
            return get().loadScenes(sceneNames, onComplete);
        }

        static public void removeScene(string sceneName)
        {
            Debug.Log("unloading <b>" + sceneName + "</b>");
            SceneManager.UnloadSceneAsync(sceneName);
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
            if (obj.name.StartsWith("~"))
            {
                Debug.Log("   <b>removing guide</b> of name : " + obj.name);
                GameObject.Destroy(obj.gameObject);
                return true;
            }

            int i = 0;
            while (i < obj.childCount)
            {
                if (!removeGuides(obj.GetChild(i))) i++;
            }

            return false;
        }

        static protected string getActiveSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        static public bool doActiveSceneNameContains(string nm)
        {
            string scName = getActiveSceneName();
            //Debug.Log(scName + " vs " + nm);
            return scName.Contains(nm);
        }

        static public bool isGameScene()
        {
            return getActiveSceneName().StartsWith("game");
            //return doActiveSceneNameContains("game");
        }

        static protected bool isResourceScene()
        {
            return doActiveSceneNameContains("resource-");
        }

        static protected bool isSceneLevel()
        {
            return doActiveSceneNameContains("level-");
        }

        static public EngineLoader get()
        {
            if (loader == null) create();
            return loader;
        }

        static public bool isLoading()
        {
            return loader != null;
        }

        /// <summary>
        /// loaded or loading (but called to be loaded at least)
        /// </summary>
        /// <param name="endName"></param>
        /// <returns></returns>
        static public bool isSceneAdded(string endName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene sc = SceneManager.GetSceneAt(i);
                //Debug.Log(sc.name + " , valid ? " + sc.IsValid() + " , loaded ? " + sc.isLoaded);
                if (sc.name.Contains(endName))
                {
                    return true;
                }
            }

            return false;
        }

#if UNITY_EDITOR

        static public bool isSceneInBuildSettingsList(string scName)
        {
            bool found = true;

            found = false;

            UnityEditor.EditorBuildSettingsScene[] scenes = UnityEditor.EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                //UnityEditor.SceneManagement.EditorSceneManager.GetSceneByBuildIndex()
                if (scenes[i].path.Contains(scName)) found = true;
            }

            return found;
        }

#endif


        static public string isContextEngineCompatible()
        {
            string[] filters = new string[] { "#" };

            for (int i = 0; i < filters.Length; i++)
            {
                if (getActiveSceneName().StartsWith(filters[i]))
                {
                    return filters[i];
                }
            }

            return "";
        }

        static public bool checkIfCanBeLoaded(string sceneLoad)
        {
            bool checkIfExists = false;

            //Debug.Log("count ? "+ SceneManager.sceneCountInBuildSettings);

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);

                //Debug.Log(path);

                if (path.Contains(sceneLoad)) checkIfExists = true;
            }

            return checkIfExists;
        }
    }

}
