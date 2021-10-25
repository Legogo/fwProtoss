using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// WON'T AUTO BOOT in some scenes (precheck and prefix ~#)
/// 
/// create
/// load engine
/// wait for feeders & co
/// destroy
/// 
/// as long as its instance exists == loading ...
/// </summary>

namespace scaffolder.engineer
{
    public class EngineStartup : MonoBehaviour
    {
        // permet de savoir si le moteur est actif
        //devient true quand on passe le check de compat
        static public bool compatibility = false;

        static protected EngineStartup eStartupInstance = null;

        [RuntimeInitializeOnLoadMethod]
        static protected void create()
        {
            string filter = isContextEngineCompatible();

            if (filter.Length > 0)
            {
                Debug.LogWarning("won't load engine here : scene starts with prefix : <b>" + filter + "</b>");

                //EngineManager.create();

                return;
            }

            init();
        }

        static public void init()
        {
            compatibility = true;
            Debug.Log("{engineer} is ON");

            if (eStartupInstance != null) return;

            eStartupInstance = new GameObject("[startup]").AddComponent<EngineStartup>();
            eStartupInstance.startupProcess();
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
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
            //Scene are not flagged as loaded during frame 1
            yield return null;
            yield return null;
            yield return null;

            //EngineLoader.loadScene("screen-loading");
            //ScreenLoading.create(false);
            ScreenLoading.showLoadingScreen();

            //Debug.Log(getStamp() + " waiting for loading screen");
            //attendre l'écran de loading
            //while (ScreenLoading.get() == null) yield return null;

            //Debug.Log(getStamp() + " loading screen should be visible, frame : " + Time.frameCount);

            string engineSceneName = EngineLoader.prefixResource + "engine";

            //Debug.Log(getStamp() + " triggering feeders ...");

            // then we load engine, to get the feeder script
            co = EngineLoader.loadScenes(new string[] { engineSceneName },
              delegate () { co = null; });

            Debug.Log(getStamp() + " waiting for engine scene ...");

            while (co != null) yield return null;

            //NEEDED if not present
            //must be created after the (existing ?) engine scene is loaded (doublon)
            //EngineManager.create();

            //safe check for engine scene presence
            Scene engineScene = SceneManager.GetSceneByName(engineSceneName);
            Debug.Assert(engineScene.IsValid());

            while (!engineScene.isLoaded) yield return null;
            yield return null;

            Debug.Log(getStamp() + " triggering feeders ...");

            // les feeders qui sont déjà présents quand on lance le runtime (pas par un load)
            EngineLoaderFeederBase[] feeders = GameObject.FindObjectsOfType<EngineLoaderFeederBase>();
            Debug.Log(getStamp() + " " + feeders.Length + " feeders still running");
            for (int i = 0; i < feeders.Length; i++)
            {
                //feeders[i].feed(gameObject.scene);
                if (!feeders[i].isFeeding()) feeders[i].feed();
            }

            //tant qu'on a des loaders qui tournent ...
            while (EngineLoader.areAnyLoadersRunning()) yield return null;

            Debug.Log(getStamp() + " is done at frame " + Time.frameCount + ", removing gameobject");

            if (engineScene.rootCount <= 0)
            {
                SceneManager.UnloadSceneAsync(engineScene);
            }

            EngineBoot booter = GameObject.FindObjectOfType<EngineBoot>();
            if(booter == null)
            {
                Debug.LogWarning(getStamp() + " no booter found ?");
            }
            else
            {
                booter?.loadingCompleted();
            }

            yield return null;

            //temp
            ScreenLoading.hideLoadingScreen();

            yield return null;

            GameObject.Destroy(gameObject);
        }

        static public string isContextEngineCompatible()
        {
            string[] filters = new string[] { "~", "#", "network", "precheck" };

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
            return "<color=#081365>" + GetType().ToString() + "</color>";
        }


        static public bool instanceExist()
        {
            //if (eStartupInstance == null) eStartupInstance = GameObject.FindObjectOfType<EngineStartup>();
            return eStartupInstance != null;
        }
    }
}