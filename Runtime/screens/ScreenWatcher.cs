using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace fwp.engine.screens
{
    /// <summary>
    /// meant to track opening/closing of a screen
    /// </summary>
    public class ScreenWatcher : ResourceWatcher
    {

        static public ScreenWatcher create(string targetScreen, Action onOpened = null, Action onCompletion = null)
        {
            ScreenWatcher tsw = new GameObject("{temp-" + UnityEngine.Random.Range(0, 10000) + "}").AddComponent<ScreenWatcher>();
            tsw.launch(targetScreen, onOpened, onCompletion);
            return tsw;
        }

        public ScreenAnimated screen;

        protected override IEnumerator resourceCreate(Action onCompletion)
        {
            bool loading = true;

            ScreensManager.open(resourceName, delegate (ScreenObject screen)
            {
                loading = false;
                this.screen = (ScreenAnimated)screen;
                Debug.Assert(this.screen != null, "null screen ? not animated screen ?");

                //Debug.Log($"{resourceName} screen opened");

                Debug.Assert(screen != null);
            });

            Debug.Log(" ... waiting for screen to be loaded ...");
            while (loading) yield return null;

            onCompletion?.Invoke();
        }

        protected override IEnumerator resourceOpen(Action onCompletion)
        {
            while (screen == null) yield return null;

            //at least one canvas visible
            while (!screen.isVisible()) yield return null;

            onCompletion?.Invoke();
        }

        protected override IEnumerator resourceClose(Action onCompletion)
        {
            Debug.Log(" ... wait for closing ...");
            while (screen.isClosing()) yield return null;

            Debug.Log(" ... wait while still flagged as opened ...");
            while (screen.isOpen()) yield return null;

            onCompletion?.Invoke();
        }

        protected override IEnumerator resourceDestroy(Action onCompletion)
        {
            while (screen != null) yield return null;

            onCompletion?.Invoke();
        }

        //abstract protected void onScreenLoaded();
        //abstract protected void onScreenClosing();

        IEnumerator process(string targetScreen, Action<ScreenAnimated> onOpened = null, Action onCompletion = null)
        {
            yield return null;
            yield return null;
            yield return null;

            Debug.Log(" ... waiting for screen to be done...");
            while (screen != null) yield return null;

            Debug.Log(" ... popup is done, continuing with flow ...");

            onCompletion?.Invoke();

            yield return null;

            GameObject.Destroy(gameObject);
        }

    }

}
