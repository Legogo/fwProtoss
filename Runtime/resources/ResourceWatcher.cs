using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace fwp.engine.scaffolder.engineer
{

    /// <summary>
    /// meant to track opening/closing of a screen
    /// </summary>
    abstract public class ResourceWatcher : MonoBehaviour
    {
        public bool verbose = false;

        protected string resourceName;

        protected Action onOpened;
        protected Action onCompletion;

        public ResourceWatcher launch(string targetResource, Action onOpened = null, Action onCompletion = null)
        {
            resourceName = targetResource;

            this.onOpened = onOpened;
            this.onCompletion = onCompletion;

            StartCoroutine(globalProcess());

            return this;
        }

        abstract protected IEnumerator resourceCreate();
        abstract protected IEnumerator resourceOpen();
        abstract protected IEnumerator resourceClose();
        abstract protected IEnumerator resourceDestroy();

        IEnumerator globalProcess()
        {
            yield return null;
            yield return null;
            yield return null;

            Coroutine co = null;

            if(verbose)
                Debug.Log(" ... waiting for creation ...");

            co = StartCoroutine(resourceCreate());
            while (co != null) yield return null;

            if (verbose)
                Debug.Log(" ... waiting for opening ...");

            co = StartCoroutine(resourceOpen());
            while (co != null) yield return null;
            onOpened?.Invoke();
            
            if (verbose)
                Debug.Log(" ... waiting for closing ...");

            co = StartCoroutine(resourceClose());
            while (co != null) yield return null;

            if (verbose)
                Debug.Log(" ... waiting for removal ...");

            co = StartCoroutine(resourceDestroy());
            while (co != null) yield return null;

            onCompletion?.Invoke();

            //remove watcher
            GameObject.Destroy(gameObject);
        }

    }
}