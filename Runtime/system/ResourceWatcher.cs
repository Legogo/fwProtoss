using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// meant to track opening/closing of a screen
/// </summary>
abstract public class ResourceWatcher : MonoBehaviour
{
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

    abstract protected IEnumerator resourceCreate(Action onCreation);
    abstract protected IEnumerator resourceOpen(Action onOpened);
    abstract protected IEnumerator resourceClose(Action onClosed);
    abstract protected IEnumerator resourceDestroy(Action onDestroyed); // screen is closed and removed

    IEnumerator globalProcess()
    {
        yield return null;
        yield return null;
        yield return null;

        //Coroutine co = null;

        Debug.Log(" ... waiting for creation ...");

        bool wait;

        wait = true;
        StartCoroutine(resourceCreate(()=> { wait = false; }));
        while (wait) yield return null;

        Debug.Log(" ... waiting for opening ...");

        wait = true;
        StartCoroutine(resourceOpen(() => { wait = false; }));
        while (wait) yield return null;

        onOpened?.Invoke();

        Debug.Log(" ... waiting for closing ...");

        wait = true;
        StartCoroutine(resourceClose(() => { wait = false; }));
        while (wait) yield return null;

        Debug.Log(" ... waiting for removal ...");

        wait = true;
        StartCoroutine(resourceDestroy(() => { wait = false; }));
        while (wait) yield return null;
        
        onCompletion?.Invoke();

        //remove watcher
        GameObject.Destroy(gameObject);
    }

}
