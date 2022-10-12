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
            return obj.AddComponent<ScreenWrapper>().setup(screenName);
        }

        ScreenObject screen;

        Action onLoaded;
        Action onEnded;

        public ScreenWrapper setup(string nm, Action onLoaded = null, Action onEnded = null)
        {
            this.onLoaded = onLoaded;
            this.onEnded = onEnded;

            StartCoroutine(processWrapper(nm));

            return this;
        }

        IEnumerator processWrapper(string nm)
        {
            screen = ScreensManager.open(nm, (ScreenObject screen) =>
            {
                this.screen = screen;
            });

            // wait for the screen
            while (screen == null) yield return null;

            this.onLoaded?.Invoke();

            // wait for screen to close
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
