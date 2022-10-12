using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using fwp.engine.scaffolder.engineer;

namespace fwp.engine
{
    /// <summary>
    /// associer autour d'une UID un ensemble de scene
    /// multi layering scenes
    /// </summary>
    public class SceneProfil
    {
        const string scene_camera = "resource-camera-procamera2D";
        const string scene_dayCycle = "day-cycle";
        const string scene_debug = "debug";

        public string uid;

        public List<string> layers = new List<string>();
        public List<string> deps = new List<string>();

        public bool loadCamera;
        public bool loadDayCycle;
        public bool loadDebug;

        Scene[] _buffScenes;

        public SceneProfil(string uid)
        {
            if (uid.ToLower().Contains("SceneManagement"))
            {
                Debug.LogError("invalid uid : " + uid);
                return;
            }

            this.uid = uid;

            // common to all
            loadCamera = true;
            loadDayCycle = true;

            // specs deps
            if (this.uid.Contains("sim-"))
            {
                //...
            }

            reload();
        }

        /// <summary>
        /// pile de toutes les scènes qui seront a charger au runtime
        /// </summary>
        public void reload()
        {
            if (layers == null) layers = new List<string>();
            else layers.Clear();

            layers.Add(uid);

            if (loadCamera) deps.Add(scene_camera);
            if (loadDayCycle) deps.Add(scene_dayCycle);
            if (loadDebug) deps.Add(scene_debug);
        }

#if UNITY_EDITOR
        public void editorLoad()
        {
            Debug.Log($"SceneProfil:editorLoad <b>{uid}</b>");

            //first load base scene
            string baseScene = layers[0];
            SceneLoaderEditor.loadScene(baseScene, UnityEditor.SceneManagement.OpenSceneMode.Single);

            //load additive others
            for (int i = 1; i < layers.Count; i++)
            {
                SceneLoaderEditor.loadScene(layers[i]);
            }

            //load deps
            for (int i = 0; i < deps.Count; i++)
            {
                SceneLoaderEditor.loadScene(deps[i]);
            }

            //lock by editor toggle
            //HalperEditor.upfoldNodeHierarchy();
        }
#endif

        public void buildLoad(System.Action<Scene> onLoadedCompleted)
        {

            fwp.engine.scenes.SceneLoader.loadScenes(deps.ToArray(), (Scene[] scs) =>
            {
                fwp.engine.scenes.SceneLoader.loadScenes(layers.ToArray(),
                (Scene[] scs) =>
                {
                    if (scs.Length <= 0)
                    {
                        Debug.LogError("no scenes returned ?");
                        for (int i = 0; i < layers.Count; i++)
                        {
                            Debug.Log("  " + layers[i]);
                        }
                    }

                    _buffScenes = scs;
                    onLoadedCompleted?.Invoke(extractMainScene());
                });
            });

        }

        public void buildUnload(System.Action onUnloadCompleted)
        {
            Debug.Log(GetType()+" : " + uid + " is <b>unloading</b>");

            fwp.engine.scenes.SceneLoader.unloadScenes(layers.ToArray(), onUnloadCompleted);
        }

        public Scene extractMainScene()
        {
            Debug.Assert(_buffScenes.Length > 0, "buff scenes must not be empty here");
            Debug.Assert(_buffScenes[0].IsValid());

            return _buffScenes[0];
        }

        /// <summary>
        /// EDITOR
        /// make sure all related scenes are present in build settings
        /// </summary>
        void forcePresenceBuildSettings()
        {

        }

        /// <summary>
        /// RUNTIME
        /// est-ce que ce profil est dispo dans les builds settings
        /// </summary>
        /// <returns></returns>
        bool isAvailableInBuild()
        {
            return true;
        }
    }

}
