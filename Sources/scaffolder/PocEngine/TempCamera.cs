using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// for temporary (loading) camera
/// </summary>

namespace scaffolder.pocEngine
{
    public class TempCamera : MonoBehaviour
    {
        AudioListener al;

        void Awake()
        {
            al = GetComponent<AudioListener>();

            checkForDoubleListener();
        }

        IEnumerator Start()
        {
            while (EngineManager.get() == null) yield return null;

            EngineManager.get().onLoadingDone += loadingDone;
        }

        public void loadingDone()
        {
            StopAllCoroutines();
            GameObject.Destroy(gameObject);
        }

        private void OnDestroy()
        {
            EngineManager.get().onLoadingDone -= loadingDone;
        }

        private void Update()
        {
            checkForDoubleListener();
        }

        void checkForDoubleListener()
        {

            if (al != null)
            {
                AudioListener[] listeners = GameObject.FindObjectsOfType<AudioListener>();
                if (listeners.Length > 1)
                {
                    GameObject.Destroy(gameObject);
                }
            }

        }

    }

}
