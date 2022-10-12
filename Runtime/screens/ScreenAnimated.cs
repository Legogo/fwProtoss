﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ces écrans ne doivent pas avoir de lien fort avec le maze
/// ils doivent etre TOUS load/unload dynamiquement en fonction des besoins
/// ex : on a pas de raison de les faire réagir au setup de la map
/// </summary>

namespace fwp.engine.screens
{
    abstract public class ScreenAnimated : ScreenObject
    {
        static public List<ScreenAnimated> openedAnimatedScreens = new List<ScreenAnimated>();

        protected Animator _animator;

        Coroutine _coprocOpening;   // opening
        Coroutine _coprocClosing;   // closing
        bool _opened = false;       // interactable

        const string PARAM_OPEN = "open";
        const string STATE_CLOSED = "closed";
        const string STATE_OPENED = "opened";

        //const string STATE_HIDING = "hiding";
        //const string STATE_OPENING = "opening";

        protected override void build()
        {
            base.build();

            _animator = GetComponent<Animator>();
            if (_animator == null) _animator = transform.GetChild(0).GetComponent<Animator>();

            //Debug.Assert(_animator != null, "screen animated animator missing ; voir avec andre");

            openedAnimatedScreens.Add(this);
        }

        virtual protected bool autoOpeningOnSetup() => true;

        protected override void setupDebug()
        {
            base.setupDebug();

            ScreenLoading.hideLoadingScreen(); // laby screen debug

            //openAnimated(); // debug setup
        }

        protected override void setup()
        {
            base.setup();

            if (autoOpeningOnSetup())
            {
                openAnimated();
            }
        }

        protected override void onScreenDestruction()
        {
            base.onScreenDestruction();

            openedAnimatedScreens.Remove(this);
        }

        virtual public void openAnimated()
        {
            if (verbose) Debug.Log(getStamp() + " open animating TAB : " + name, transform);

            //already animating ?
            if (_coprocOpening != null)
            {
#if UNITY_EDITOR
                Debug.LogError(getStamp() + " => open animated => coroutine d'opening tourne déjà ?");
#else
      Debug.LogWarning(getStamp() + " => open animated => coroutine d'opening tourne déjà ?");
#endif
            }

            _coprocOpening = StartCoroutine(processAnimatingOpening());
        }

        virtual protected void setupBeforeOpening()
        {
            toggleVisible(false); // hide for setup
        }

        IEnumerator processAnimatingOpening()
        {
            setupBeforeOpening();

            toggleVisible(true);

            ScreenLoading.hideLoadingScreen(); // laby screen, now animating open screen

            if (_animator != null)
            {
                _animator.SetBool(PARAM_OPEN, true);

                //animator state change...
                yield return null;
                yield return null;
                yield return null;

                //... do something spec for animating screen
                //while (_animator.GetCurrentAnimatorStateInfo(0).IsName(STATE_OPENING)) yield return null;
                while (!_animator.GetCurrentAnimatorStateInfo(0).IsName(STATE_OPENED)) yield return null;
            }

            evtOpeningAnimationDone();
        }

        /// <summary>
        /// do something at the end of opening animation
        /// </summary>
        virtual protected void evtOpeningAnimationDone()
        {
            _coprocOpening = null;

            toggleVisible(true); // jic

            _opened = true;

            if (verbose) Debug.Log(getStamp() + " OPENED");
        }

        /// <summary>
        /// for UI buttons
        /// </summary>
        public void actionClose()
        {
            closeAnimated();
        }

        virtual public void closeAnimated()
        {
            if (isClosing())
            {
                Debug.LogWarning(" ... already closing");
                return;
            }

            if (verbose) Debug.Log(getStamp() + " CLOSING ...");

            _coprocClosing = StartCoroutine(processAnimatingClosing());
        }

        virtual protected void setupBeforeClosing()
        {
            _opened = false;
        }

        IEnumerator processAnimatingClosing()
        {
            yield return null; // laisser le temps a coprocClosing d'etre assigné :shrug:

            setupBeforeClosing();

            yield return null;
            yield return null;
            yield return null;

            if (_animator != null)
            {
                _animator.SetBool(PARAM_OPEN, false);

                Debug.Log("waiting for screen to close");

                while (!_animator.GetCurrentAnimatorStateInfo(0).IsName(STATE_CLOSED)) yield return null;
            }

            evtClosingAnimationCompleted();

            unload();
        }

        /// <summary>
        /// après l'anim de fermeture
        /// </summary>
        virtual protected void evtClosingAnimationCompleted()
        {
            _opened = false; // jic
            _coprocClosing = null;
        }

        /// <summary>
        /// /! 
        /// APRES anim open
        /// AVANT anim close
        /// </summary>
        public bool isOpen() => _opened;

        public bool isOpening() => _coprocOpening != null;
        public bool isClosing() => _coprocClosing != null;

        /// <summary>
        /// something above ?
        /// </summary>
        virtual protected bool isInteractable() => _opened;

        static public ScreenAnimated getScreen(string screenName)
        {
            ScreenAnimated[] scs = GameObject.FindObjectsOfType<ScreenAnimated>();
            for (int i = 0; i < scs.Length; i++)
            {
                if (scs[i].isScreenOfSceneName(screenName)) return scs[i];
            }
            return null;
        }

        static public ScreenAnimated toggleScreen(string scNameToOpen)
        {
            ScreenObject so = ScreensManager.getScreen(scNameToOpen);
            Debug.Log(so);

            ScreenAnimated sa = so as ScreenAnimated;
            Debug.Log(sa);

            if (sa.isOpen())
            {
                sa.closeAnimated();
                return sa;
            }

            Debug.LogWarning("toggling " + scNameToOpen + " did nothing ; screen is opening ?");

            return null;
        }

    }

}
