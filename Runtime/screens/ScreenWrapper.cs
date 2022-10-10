using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace fwp.engine.screens
{

    /// <summary>
    /// loading
    /// open
    /// do something
    /// close
    /// unload
    /// </summary>
    public class ScreenWrapper : MonoBehaviour
    {
        static public ScreenWrapper call(string screenName)
        {
            GameObject obj = new GameObject("~sw-" + screenName);
            return obj.AddComponent<ScreenWrapper>();
        }

        ScreenObject screen;

        Action onLoaded;
        Action onEnded;

        public ScreenWrapper setup(string nm, Action onLoaded, Action onEnded)
        {
            this.onLoaded = onLoaded;
            this.onEnded = onEnded;

            StartCoroutine(processLoading(nm));

            return this;
        }

        IEnumerator processLoading(string nm)
        {
            screen = ScreensManager.open(nm, () =>
            {
                this.screen = ScreensManager.getScreen(nm);
            });

            while (screen == null) yield return null;

            this.onLoaded?.Invoke();

            StartCoroutine(processOpened());
        }

        IEnumerator processOpened()
        {
            while(this.screen != null) yield return null;

            this.onEnded?.Invoke();

            GameObject.Destroy(gameObject);
        }

        public bool isScreenExisting()
        {
            return screen != null;
        }

        public bool isScreenOpened()
        {
            return screen.isVisible();
        }
    }

}
